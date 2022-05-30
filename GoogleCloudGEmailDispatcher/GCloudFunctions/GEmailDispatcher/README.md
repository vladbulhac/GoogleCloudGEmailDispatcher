# EmailDispatcher Cloud Function configuration
> Entry point: EmailDispatcher.Function
##### Sample.csproj example:
```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp3.1</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Google.Cloud.Functions.Hosting" Version="1.0.0" />
    <PackageReference Include="Google.Apis" Version="1.56.0" />
	<PackageReference Include="Google.Apis.Auth" Version="1.56.0" />
	<PackageReference Include="Google.Apis.Gmail.v1" Version="1.56.0.2622" />
	<PackageReference Include="Google.Cloud.Datastore.V1" Version="3.3.0" />
	<PackageReference Include="Google.Cloud.Functions.Framework" Version="1.0.0" />
	<PackageReference Include="MimeKit" Version="3.2.0" />
  </ItemGroup>
</Project>
```

##### Required Environment Variables:
- GCLOUD_FUNCTION_REGION
- GCLOUD_FUNCTION_PROJECT_ID
- NAMEOF_OAUTHINIT_GCLOUD_FUNCTION
- GCLOUD_DATASTORE_TOKENKEY (can be any string you want, e.g OAuthToken)
- GMAIL_CLIENT_ID
- GMAIL_CLIENT_SECRET