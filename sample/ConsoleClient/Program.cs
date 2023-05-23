using IdentityModel.Client;

var client = new HttpClient();

var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5001");
if (disco.IsError)
{
    Console.WriteLine(disco.Error);
}


// request token
var tokenResponse = await client.RequestClientCredentialsTokenAsync(
    new ClientCredentialsTokenRequest
    {
        Address = disco.TokenEndpoint,
        ClientId = "console-client",
        ClientSecret = "Lem0nC0deSecret",
        Scope = "chat-api"
    });

if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
}

Console.WriteLine(tokenResponse.AccessToken);
Console.ReadLine();