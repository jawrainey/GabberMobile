# Gabber Mobile

> Gabber mobile applications written in Xamarin using native Android and iOS UIs and shared codebase via PCL.

## Project Structure

| Path | Info |
| ---- | ---- |
| `/Gabber.Android`  | The Gabber Android mobile application codebase |
| `/Gabber.iOS`      | The Gabber iOS mobile application codebase |
| `/Gabber.PCL`      | Shared codebase between Android/iOS, i.e. [RESTClient](https://github.com/jawrainey/GabberMobile/blob/master/GabberPCL/RestClient.cs), [Database](https://github.com/jawrainey/GabberMobile/blob/master/GabberPCL/Session.cs), [Localisation](??), and [DI](https://github.com/jawrainey/GabberMobile/tree/master/GabberPCL/Interfaces) |

### Running locally

Open [Gabber.sln](https://github.com/jawrainey/GabberMobile/blob/master/Gabber.sln) with Visual Studio and install the package dependencies. To use a local version of the [GabberAPI](https://github.com/jawrainey/GabberAPI) update `BaseAddress` in RESTClient to either use your local IP (as it runs on `0.0.0.0:5000` locally) or create an [ngrok](https://ngrok.com/)tunnel to expose the API.
