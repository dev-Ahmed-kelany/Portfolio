using Microsoft.OpenApi;
using Npgsql;
using Portfolio.Business;
using Portfolio.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Initialize connection string from configuration
clsSettings.SetConnectionString(builder.Configuration);

// Add services to the container.

// DataSource
//builder.Services.AddSingleton<NpgsqlDataSource>(sp =>
//{
//    return NpgsqlDataSource.Create(clsSettings.ConnectionString);
//});

//builder.Services.AddNpgsqlDataSource(clsSettings.ConnectionString);

builder.Services.AddNpgsqlDataSource(clsSettings.ConnectionString);

// Repositories (Data Access)
builder.Services.AddScoped<IPerson, clsPersonDA>();
builder.Services.AddScoped<IAbout, clsAboutDA>();
builder.Services.AddScoped<IContactInfo, clsContactInfoDA>();
builder.Services.AddScoped<IExperience, clsExperienceDA>();
builder.Services.AddScoped<IJobTitle, clsJobTitleDA>();
builder.Services.AddScoped<ISkill, clsSkillDA>();
builder.Services.AddScoped<ISkillCategory, clsSkillCategoriesDA>();
builder.Services.AddScoped<IPersonSkill, clsPersonSkillDA>();
builder.Services.AddScoped<IProjectSkill, clsProjectSkillDA>();
builder.Services.AddScoped<IProject, clsProjectDA>();
builder.Services.AddScoped<ISocialLink, clsSocialLinkDA>();
builder.Services.AddScoped<IUser, clsUserDA>();
builder.Services.AddScoped<ISkillQueries, clsSkillQueriesDA>();

// Business Services
builder.Services.AddScoped<clsPerson>();
builder.Services.AddScoped<clsAbout>();
builder.Services.AddScoped<clsContactInfo>();
builder.Services.AddScoped<clsExperience>();
builder.Services.AddScoped<clsJobTitle>();
builder.Services.AddScoped<clsSkill>();
builder.Services.AddScoped<clsSkillCategory>();
builder.Services.AddScoped<clsPersonSkill>();
builder.Services.AddScoped<clsProjectSkill>();
builder.Services.AddScoped<clsProject>();
builder.Services.AddScoped<clsSocialLink>();
builder.Services.AddScoped<clsUser>();
builder.Services.AddScoped<clsSkillQueries>();

// builder.Services.AddScoped<DbService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


