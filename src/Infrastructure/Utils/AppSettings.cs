using System;
namespace Blogs.Infrastructure.Utils;

public class AppSettings
{
    public int TokenLifeTime { get; set; } = default!;

    public string ValidAudience { get; set; } = default!;

    public string ValidIssuer { get; set; } = default!;

    public string Secret { get; set; } = default!;

}
