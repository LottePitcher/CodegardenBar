# CodegardenBar
## Local dev setup
### Prerequisites
- A way to tunnel the backend to the outside world so twitter can send webhook requests (https://ngrok.com/)
- Twitter credentials to the app and owning account you want to use for this virtual bar
- Twitter Account Activity API environment configured (https://developer.twitter.com/en/account/environments)
- Postman if you want to import the collection for easy api testing (enter collection url here)
- A way to serve a static website (https://www.npmjs.com/package/live-server)

### Setup configuration
- Add a new file in the root of the backend app named `appsetttings.json.local` with all necesary crendetials
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
  }
}
```
- If using postman, edit the collection variables with your host(baseUrl) and admin passwords

### Running the app
- Serve the static website files that hold the app that shows the bartending (https://github.com/LottePitcher/CGBarFrontend)
- Open the visual studio project and run the backend project, it should open a webpage with an https url
- Run your tunneling software (ngrok) on that https url, you should get a url like this `https://3e3c31de0a42.ngrok.io`

### Register twitter webhooks
- Register your tunnel url by sending a get request (with adminPassword header) to /TwitterAdmin/RegisterWebhook (or use postman)
- Request an appAuthorization Pin by sending a get request (with adminPassword header) to /TwitterAdmin/StartSubscribeToAccount (or use postman)
- Browse the twitter url in that response and obtain the pin
- Finish registration by sending a get request (with adminPassword header) to /TwitterAdmin/StartSubscribeToAccount (or use postman) with authkey paramater to the key from the 2nd response and pin paramter to the pin obtained from twitter

### To do every time your tunnel url changes
- Update appsetttings.json.local host
- Update postman collection baseUrl
- Rerun twitter registration
