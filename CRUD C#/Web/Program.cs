using Business;
using Data;
using Date;
using Entity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.SqlServer.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  Comandos para hacer la migracion 
//Remove - Migration - Project Entity - StartupProject Web1
//Add-Migration NuevaMigracion -Project Entity -StartupProject Web1
//Update-Database -Project Entity -StartupProject Web1

// Registrar clases de Person

// Registrar clases de Form
builder.Services.AddScoped<BranchData>();
builder.Services.AddScoped<BranchBusiness>();

builder.Services.AddScoped<CategoryData>();
builder.Services.AddScoped<CategoryBusiness>();

builder.Services.AddScoped<CompanyData>();
builder.Services.AddScoped<CompanyBusiness>();

builder.Services.AddScoped<FormData>();
builder.Services.AddScoped<FormBusiness>();

builder.Services.AddScoped<FormModuleData>();
builder.Services.AddScoped<FormModuleBusiness>();

builder.Services.AddScoped<ImagenItemData>();
builder.Services.AddScoped<ImagenItemBusiness>();

builder.Services.AddScoped<InventaryDetailsData>();
builder.Services.AddScoped<InventaryDetailsBusiness>();

builder.Services.AddScoped<ItemData>();
builder.Services.AddScoped<ItemBusiness>();

builder.Services.AddScoped<LogActivityData>();
builder.Services.AddScoped<LogActivityBusiness>();

builder.Services.AddScoped<ModuleData>();
builder.Services.AddScoped<ModuleBusiness>();

builder.Services.AddScoped<PermissionData>();
builder.Services.AddScoped<PermissionBusiness>();

builder.Services.AddScoped<PersonData>();
builder.Services.AddScoped<PersonBusiness>();

builder.Services.AddScoped<RolData>();
builder.Services.AddScoped<RolBusiness>();

builder.Services.AddScoped<RolFormPermissionData>();
builder.Services.AddScoped<RolFormPermissionBusiness>();

builder.Services.AddScoped<RolUserData>();
builder.Services.AddScoped<RolUserBusiness>();

builder.Services.AddScoped<UserData>();
builder.Services.AddScoped<UserBusiness>();

builder.Services.AddScoped<ZoneData>();
builder.Services.AddScoped<ZoneBusiness>();



// Agregar DbContext conexion a base de datos
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer("name=DefaultConnection"));

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