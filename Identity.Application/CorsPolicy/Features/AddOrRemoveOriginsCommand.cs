namespace Identity.Application.CorsPolicy.Features;

public class AddOrRemoveOriginsCommand
{
    public List<string> Origins { get; set; } = [];
}