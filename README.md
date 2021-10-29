# CodegardenBar

## Local dev setup

### Prerequisites

- A way to tunnel the backend to the outside world so twitter can send webhook requests (https://ngrok.com/)
- Twitter credentials to the app and owning account you want to use for this virtual bar
- Twitter Account Activity API environment configured (https://developer.twitter.com/en/account/environments)
- Postman if you want to import the collection for easy api testing (https://www.getpostman.com/collections/46c767cfb85ef49207c2)
- A way to serve a static website (https://www.npmjs.com/package/live-server)

### Setup configuration

- Add a new file in the root of the backend app named `appsetttings.json.local` with all necessary credentials
```json
{
  "TwitterApi": {
    "ConsumerKey": "",
    "ConsumerKeySecret": "",
    "ApplicationBearerToken": "",
    "AccessToken": "",
    "AccessTokenSecret": "",
    "Host": "https://3e3c31de0a42.ngrok.io/",
    "Environment": "",
    "AdminPassword": "" //used to contact internal apis
  },
  "BarTender": {
    "AdminPassword": "" //used to contact internal apis
  },
  "DiscordBotToken" : "",
  "DiscordBotPrefix" : "!bar "
}
```
- If using postman, edit the collection variables with your host(baseUrl) and admin passwords

### Running the app

- Serve the static website files that hold the app that shows the bartending (https://github.com/LottePitcher/CGBarFrontend)
- Open the visual studio project and run the backend project, it should open a webpage with an https url. Append `/status/ping` to the url to confirm it's working (there is no default page).
- Run your tunneling software (ngrok) on that https url, you should get a url like this `https://3e3c31de0a42.ngrok.io`
- Ensure that the static website is using the correct `baseUrl`

### Register Twitter webhooks

- Register your tunnel url:
  - send a get request (with adminPassword header) to `/TwitterAdmin/RegisterWebhook` (or use postman)
- Request an appAuthorization Pin:
  - send a get request (with adminPassword header) to `/TwitterAdmin/StartSubscribeToAccount` (or use postman)
  - browse the twitter url in that response (having logged in to the correct twitter account first) to obtain the pin
- Finish registration:
  - send a get request (with adminPassword header) to `/TwitterAdmin/SubscribeToAccount` (or use postman) using:
    - authkey: the key from the `StartSubscribeToAccount` response
    - pin: the pin obtained from twitter url

### To do every time your tunnel url changes

- Update appsettings.json.local Host 
- Update postman collection baseUrl
- Rerun Twitter webhooks registration process
