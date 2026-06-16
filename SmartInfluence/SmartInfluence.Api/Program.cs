using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SmartInfluence.Data;
using SmartInfluence.Data.Interfaces;
using SmartInfluence.Data.Repositories;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

var builder = WebApplication.CreateBuilder(args);

var elasticUrl = builder.Configuration["Elasticsearch:Url"];
var elasticUsername = builder.Configuration["Elasticsearch:Username"];
var elasticPassword = builder.Configuration["Elasticsearch:Password"];
var elasticApiKey = builder.Configuration["Elasticsearch:ApiKey"];
var esIndex = builder.Configuration["Elasticsearch:DefaultIndex"] ?? "influencers";

if (string.IsNullOrWhiteSpace(elasticUrl))
{
    throw new InvalidOperationException("Elasticsearch:Url is not configured.");
}

var settings = new ElasticsearchClientSettings(new Uri(elasticUrl!))
    .DefaultIndex(esIndex);

if (!string.IsNullOrWhiteSpace(elasticApiKey))
{
    settings = settings.Authentication(new ApiKey(elasticApiKey));
}
else if (!string.IsNullOrWhiteSpace(elasticUsername) &&
         !string.IsNullOrWhiteSpace(elasticPassword))
{
    settings = settings.Authentication(
        new BasicAuthentication(elasticUsername, elasticPassword));
}

builder.Services.AddSingleton(new ElasticsearchClient(settings));
builder.Services.AddScoped<ElasticsearchService>();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureConnection")));

builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IClientInfluencerRepository, ClientInfluencerRepository>();
builder.Services.AddScoped<IInfluencerRepository, InfluencerRepository>();
builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IClientInfluencerService, ClientInfluencerService>();
builder.Services.AddScoped<IInfluencerService, InfluencerService>();
builder.Services.AddScoped<IProductQueryAiService, ProductQueryAiService>();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddSwaggerGen(opt =>
{
    opt.DocumentFilter<CustomSwaggerFilter>();

    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});
builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins(
            "http://localhost:3000",
            "https://olehskomorokha.github.io",
            "http://192.168.56.1:3000",
            "http://127.0.0.1:5500")
        .AllowAnyMethod()
        .AllowAnyHeader();
}));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("corspolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public class CustomSwaggerFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var nonMobileRoutes = swaggerDoc.Paths
            .Where(x => x.Key.ToLower().Contains("/api/"))
            .ToList();
    }

}
