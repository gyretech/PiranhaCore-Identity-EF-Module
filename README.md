## Welcome PiranhaCMS Identity EF Module
PiranhaCMS Module that handles ASP.NET Core Identity with Entity Framework as the backing store.

| Build server | Platform     | Build status |
|--------------|--------------|--------------|
| AppVeyor     | Windows      | [![Build status](https://ci.appveyor.com/api/projects/status/fuaiwkwk1kpgyya7?svg=true)](https://ci.appveyor.com/project/gyretech/piranhacore-identity-ef-module)
| Travis       | Linux / OS X | [![Build status](https://travis-ci.org/gyretech/PiranhaCore-Identity-EF-Module.svg?branch=master)](https://travis-ci.org/gyretech/PiranhaCore-Identity-EF-Module)

## Registering the Module

You register the Module by hooking it into the services. Please note that generic AspNetCore & Piranha setup has been omitted. However you should register the Module after your other Piranha CMS configurations.

    public IServiceProvider ConfigureServices(IServiceCollection services) {
      .... 
      services.AddPiranhaIdentitySecurityEF(o => 
      {
        o.ConnectionString = defaultConnection;
        o.Migrate = true;
        o.Users = new[] 
        {
          new IdentityAppUser(Piranha.Manager.Permission.All())
          { 
            UserName = "Admin", 
            Password = "P@sswOrd1", 
            FirstName = "Admin", 
            LastName = "User" 
          } 
        };
        o.EnableFirstLastNameClaim = true;
      });
      ....
    }

## Initialize the Module

Once the Module has been configured you initialize it. Please note that generic AspNetCore setup has been omitted.

    ....
    
    // Initialize the Identity EF module
    app.UsePiranhaIdentityEFSecurity();
    
    ....
    



