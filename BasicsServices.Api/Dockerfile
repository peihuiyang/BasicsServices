FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY . /publish
WORKDIR /publish
ENTRYPOINT ["dotnet", "BasicsServices.Api.dll"]
CMD ["--apiport=8009","--apiadd=106.13.10.198","--consul=http://106.13.10.198:8500"]