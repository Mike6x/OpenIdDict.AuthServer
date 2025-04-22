using System.ComponentModel;

namespace Framework.Core.Storage.File;

public enum FileType
{
    [Description(".jpg,.png,.jpeg")]
    Image,

    [Description(".xls,.xlsx")]
    Excel,

    [Description(".zip")]
    QuizMedia,

    [Description(".pdf,.doc,.zip,.rar")]
    Document
}
