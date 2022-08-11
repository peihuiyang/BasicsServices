FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY . /publish
WORKDIR /publish
ENTRYPOINT ["dotnet", "BasicsServices.Api.dll"]
CMD ["--apiadd=106.13.10.198","--apiport=8010","--consul=http://106.13.10.198:8500"]