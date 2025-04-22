using System.Collections.ObjectModel;
using Ardalis.Specification.EntityFrameworkCore;
using Framework.Core.DataIO;
using Framework.Core.Mail;
using Framework.Core.Specifications;
using Framework.Core.Storage.File;
using Framework.Core.Storage.File.Features;
using Identity.Application.Users.Dtos;
using Identity.Application.Users.Features.ExportUsers;
using Identity.Domain.Entities;
using Identity.Shared.Authorization;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.Users;

internal partial class UserService
{
    public async Task<byte[]> ExportAsync(ExportUsersRequest request, CancellationToken cancellationToken)
    {
        var spec = new EntitiesByBaseFilterSpec<AppUser>(request);

        var list = await userManager.Users
            .WithSpecification(spec)
            .ProjectToType<UserExportDto>()
            .ToListAsync(cancellationToken);

        return dataExport.ListToByteArray(list);
    }

    public async Task<ImportResponse> ImportAsync(FileUploadCommand uploadFile, bool isUpdate, string origin, CancellationToken cancellationToken)
    {
        var items = await dataImport.ToListAsync<AppUser>(uploadFile, FileType.Excel);

        ImportResponse response = new()
        {
            TotalRecords = items.Count,
            Message = ""
        };

        if (response.TotalRecords <= 0)
        {
            response.Message = "File is empty or Invalid format";
            return response;
        }

        int count = 0;
        try
        {
            if (isUpdate)
            {
                foreach (var item in items)
                {
                    var user = await userManager.FindByIdAsync(item.Id.ToString());
                    if (user != null)
                    {
                        user.FirstName = item.FirstName;
                        user.LastName = item.LastName;
                        user.UserName = item.UserName;
                        user.PhoneNumber = item.PhoneNumber;
                        user.IsActive = item.IsActive;
                        user.EmailConfirmed = item.IsActive && item.EmailConfirmed;

                        _ = await userManager.UpdateAsync(user);
                        count++;
                    }
                }

                response.Message = $"Updated {count} Users successfully";
            }
            else
            {
                foreach (var item in items)
                {
                    var result = await userManager.CreateAsync(item, item.UserName!);
                    if (result.Succeeded)
                    {
                        count++;
                        // add basic role
                        _ = await userManager.AddToRoleAsync(item, AppRoles.Basic);

                        // send confirmation mail
                        if (!string.IsNullOrEmpty(item.Email))
                        {
                            string emailVerificationUri = await GetEmailVerificationUriAsync(item, origin);
                            var mailRequest = new MailRequest(
                                new Collection<string> { item.Email },
                                "Confirm Registration",
                                emailVerificationUri);
                            _ = jobService.Enqueue("email", () => mailService.SendAsync(mailRequest, CancellationToken.None));
                        }
                    }
                }

                response.Message = $"Imported {count} Users successfully";
            }
        }
        catch (Exception)
        {
            response.Message = $"Internal error with {count} items!";
            return response;
        }

        return response;
    }
}
