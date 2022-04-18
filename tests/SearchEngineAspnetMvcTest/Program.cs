using System.Collections.Immutable;
using System.Text;
using Dncy.Tools;
using Dncy.Tools.LuceneNet;
using J2N.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Cn.Smart;
using Lucene.Net.Analysis.Util;
using Lucene.Net.Util;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IFieldSerializeProvider, NewtonsoftMessageSerializeProvider>();
builder.Services.Configure<LuceneSearchEngineOptions>(o =>
{
    o.IndexDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "luceneIndexs");
});

var defaultStop= CharArraySet.UnmodifiableSet(WordlistLoader.GetWordSet(IOUtils.GetDecodingReader(typeof(SmartChineseAnalyzer), "stopwords.txt", Encoding.UTF8), "//", LuceneSearchEngine.LuceneVersion));
var stopwords = new CharArraySet(LuceneSearchEngine.LuceneVersion, new string[] {"��", "sb"}, true);
stopwords.Add(defaultStop.ToImmutableArray());

builder.Services.AddScoped<Analyzer>(s => new SmartChineseAnalyzer(LuceneSearchEngine.LuceneVersion,stopwords));
builder.Services.AddScoped<LuceneSearchEngine>();


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
    private bool disposedValue;

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


    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: �ͷ��й�״̬(�йܶ���)
            }

            // TODO: �ͷ�δ�йܵ���Դ(δ�йܵĶ���)����д�ս���
            // TODO: �������ֶ�����Ϊ null
            disposedValue = true;
        }
    }

    // // TODO: ������Dispose(bool disposing)��ӵ�������ͷ�δ�й���Դ�Ĵ���ʱ������ս���
    // ~NewtonsoftMessageSerializeProvider()
    // {
    //     // ��Ҫ���Ĵ˴��롣�뽫���������롰Dispose(bool disposing)��������
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // ��Ҫ���Ĵ˴��롣�뽫���������롰Dispose(bool disposing)��������
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}