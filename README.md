# ChatovaciaApp

\# Chatovacia aplikácia



Projekt pozostáva z troch častí:



```text

ChatApp.Api/       ASP.NET Core Web API

ChatApp.Wpf/       WPF desktop klient

ChatApp.Shared/    Zdieľané DTO/modely medzi API a WPF klientom

```



Aplikácia používa databázu MySQL spúšťanú cez Docker Compose.



\---



\## Použité technológie



\- C#

\- ASP.NET Core Web API

\- Entity Framework Core

\- WPF

\- MySQL

\- Docker / Docker Compose



\---



\## Nastavenie `.env` súboru



V repozitári sa nachádza súbor:



```text

.env.example

```



Z neho je potrebné vytvoriť lokálny súbor:



```text

.env

```



Na Windows:



```bash

copy .env.example .env

```



Na Linux/macOS:



```bash

cp .env.example .env

```



\---



\## Spustenie aplikácie



Po vytvorení `.env` súboru spustite projekt cez Docker Compose:



```bash

docker compose up --build

```



Tým sa spustí databáza a API.



\---



\## Spustenie WPF klienta



WPF klient sa spúšťa samostatne cez Visual Studio.



Postup:



1\. Otvorte solution súbor:



```text

ChatovaciaApp.slnx

```

2\. Nastavte vo Visual Studiu ako startup projekt:



```text

ChatApp.Wpf

```



4\. Spustite WPF aplikáciu.



API musí byť spustené pred spustením WPF klienta, pretože klient komunikuje s API.



\---



Potom vo Visual Studiu spustite projekt:



```text

ChatApp.Wpf

```



\---





