using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Business;
using Portfolio.DataAccess;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Initialize connection string from configuration
clsSettings.SetConnectionString(builder.Configuration);

// Add services to the container.

// DataSource
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("PortfolioApiCorsPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7256", "http://localhost:5086")
        .AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,

            ValidateAudience = true,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,

            ValidIssuer = "PortfolioApi",

            ValidAudience = "PortfolioApiUsers",

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("THIS_IS_A_VERY_SECRET_KEY_123456"))
        };
    });


builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Register Swagger generator and customize its behavior.
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // 👈 ADD THIS
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("PortfolioApiCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


