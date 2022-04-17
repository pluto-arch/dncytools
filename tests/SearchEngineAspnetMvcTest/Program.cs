using Dncy.Tools.LuceneNet;
using Lucene.Net.Analysis.Cn.Smart;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IFieldSerializeProvider, NewtonsoftMessageSerializeProvider>();
builder.Services.Configure<LuceneSearchEngineOptions>(o =>
{
    o.Analyzer = new SmartChineseAnalyzer(LuceneSearchEngine.LuceneVersion);
    o.IndexDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "luceneIndexs");
});
builder.Services.AddSingleton<LuceneSearchEngine>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();



public class NewtonsoftMessageSerializeProvider : IFieldSerializeProvider
{
    /// <inheritdoc />
    public string Serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    /// <inheritdoc />
    public T? Deserialize<T>(string objStr)
    {
        return JsonConvert.DeserializeObject<T>(objStr);
    }

    /// <inheritdoc />
    public object? Deserialize(string objStr, Type type)
    {
        return JsonConvert.DeserializeObject(objStr, type);
    }
}