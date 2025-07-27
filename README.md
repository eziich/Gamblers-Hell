# Gambler's Hell


## Summary & Idea
Gambler's Hell is a web application inspired by the book "Inferno" by Dante.
The application consists of nine gambling games. The concept behind this collection of games is to win each one sequentially. Completing a game will unlock the next. Just like in the book "Inferno", the player must conquer all nine games, representing the nine circles of hell, in order to win. Each game has its own set of rules, which the player can read in the rules section.

---

## Technologies
- Backend: C# ASP .NET core (Blazor framework)
- Database: SQLite
- Frontend: HTML & CSS paired with MudBlazor (design component library)

---

## Features
- Register/Login
- Unverified users removal (account will be deleted if the user doesn't verify within 10 minutes)
- Email Sender System (forgotten password and verification)
- Daily Rewards System (site tracks the user's last login to reward them accordingly)
- Competitive Ranking System (featuring "Gentlemen" and "Ruler" tiers)
- Progression System (unlockable lore and access to nine playable games - users must win each game to progress to the next)
- High Rank Punishment System (users at levels 7, 8, and 9 face penalties for losing)
- Anti-Cheat System (users cannot leave or refresh pages without penalties, and cannot access higher-level games directly via URL)

---

## Required Packages 
- BCrypt.Net-Next (Server) 
- HttpClientFactory (Client)
- MailKit (Server) 
- Microsoft.AspNetCore.Components.Authorization (Client)
- Microsoft.AspNetCore.Components.WebAssembly (Client)
- Microsoft.AspNetCore.Components.Authorization.DevServer (Client)
- Microsoft.AspNetCore.Components.Authorization.Server (Server) 
- Microsoft.EntityFrameworkCore.Sqlite (Server) 
- Microsoft.EntityFrameworkCore.Tools (Server) 
- Microsoft.Extensions.Http (Client)
- Microsoft.IdentityModel.Clients.ActiveDirectory (Client)
- Microsoft.NET.ILLink.Tasks (Client)
- Microsoft.NET.Sdk.WebAssembly.Pack (Client)
- MimeKit (Server) 
- MudBlazor (Client)
- MudBlazor.ThemeManager (Client)
- Swashbuckle.AspNetCore (Server) 

---

## Disclaimers
- Gambler's Hell is a fictional web application created for entertainment and enhancement of my coding skills.
- Gambler's Hell is NOT affiliated with any real-life betting companies and does NOT involve real money or actual gambling transactions.
- For the "Email Sender Service" to work properly you will need to set your email address info in appsettings.json on server side (I used user-secrets).
- The connection string in appsettings.json on the server is also empty. Since you will get the database in the project folder, you can simply adjust the connection string path
- When you use "Email Sender" and receive mail, there is a chance the logo of the app won't be displayed properly since the logo is uploaded on Imgur. I am not sure how long imgur keeps pictures uploaded, so there is possibility the link to the GamblersHell logo is expired (depending on when you clone this repo)
---
