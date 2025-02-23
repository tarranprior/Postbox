# 📫 Postbox
![](https://img.shields.io/badge/Built_with-.NET_8.0-blue)

📫 Postbox is a lightweight encryption tool which allows users to generate key pairs, exchange public keys, encrypt and decrypt messages, and communicate securely over SMTP using the [RSA](https://en.wikipedia.org/wiki/RSA_(cryptosystem))-2048/4096 algorithm.

## Features

## Installation

1. Clone the repository:
    ```s
    git clone https://github.com/tarranprior/Postbox
    ```
    
2. Change to the directory.
    ```s
    cd Postbox
    ```

3. Restore the dependencies and run the application using `dotnet`:
    ```s
    dotnet restore
    dotnet run Postbox --help
    ```

4. You can also use 🐋 Docker by running:
   ```s
   docker build -t postbox .
   docker volume create postbox_keys
   ```

   > You must also change the `SMTP_SERVER` value in the `.env` file to `host.docker.internal`, and disable SSL with `SMTP_SSL=FALSE` (or import the SSL certificate to Docker).<br/>
   >
   > ```sh
   > SMTP_SERV=YOUR_SMTP_SERVER
   > SMTP_PORT=YOUR_SMTP_PORT
   > SMTP_SSL=TRUE
   > SMTP_USER=YOUR_EMAIL_ADDRESS
   > SMTP_PASS=YOUR_PASSWORD
   > 
   > # SMTP_SERVER=host.docker.internal # for Docker
   > # SMTP_SSL=FALSE # for Docker
   > ```

## Configuration

1. Install and configure a local SMTP Server like [Proton Bridge](https://proton.me/mail/bridge).
2. Export the certificate and install:
    ```s
    ** Import the certificate:
    PS C:\> Import-Certificate -FilePath "./cert.pem" -CertStoreLocation Cert:\CurrentUser\Root

    ** Ensure SMTP is active:
    PS C:\> Get-NetTCPConnection -LocalPort 1025
    
    LocalAddress                        LocalPort RemoteAddress
    ------------                        --------- -------------
    127.0.0.1                           1025      0.0.0.0    
    ```
3. Update the values in `.env.EXAMPLE` and rename to `.env`.
  
    ```s
    SMTP_SERV=YOUR_SMTP_SERVER * Defaults to 127.0.0.1
    SMTP_PORT=YOUR_SMTP_PORT * Defaults to 1025 
    SMTP_USER=YOUR_EMAIL_ADDRESS
    SMTP_PASS=YOUR_PASSWORD
    ```

The `SMTP_USER` value will be the name of your public and private keys: `YOUR_EMAIL_ADDRESS_public.pem` and `YOUR_EMAIL_ADDRESS_private.pem`. These will then act as the default keys when encrypting and decrypting messages.

## Usage
```
Description:
  📫 Postbox is a lightweight encryption tool which allows users to generate key pairs,
exchange public keys, encrypt and decrypt messages, and communicate securely over SMTP using RSA.

Usage:
  Postbox [command] [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  generate-keys                    Generates a new key pair.
  encrypt-message <message> <key>  Encrypts a message.
  decrypt-message <message> <key>  Decrypts a message.
  import-key <key> <email>         Imports a key from a local file.
  send-key <key> <email>           Emails a public key to a recipient.
  send-message <message> <email>   Sends an encrypted message to a recipient.
```

### Examples

```s
  generate-keys  Generates a new key pair.

  > dotnet run -- generate-keys
  > dotnet run -- generate-keys --bits 4096
```

```s
  import-key <key> <email>         Imports a key from a local file.

  > dotnet run -- import-key "path/to/key.pem" "email@example.com"
  > dotnet run -- import-key --key "path/to/key.pem" --email "email@example.com"

  ** Output:
  [00:00:00 INF] 📥 Key successfully imported for `email@example.com`.
```

```s
  send-key <key> <email>  Emails a public key to a recipient.

  > dotnet run -- send-key "recipient_email@example.com"
  > dotnet run -- send-key --key "email@example.com" --email "recipient_email@example.com"
  
  ** Output:
  [00:00:00 INF] 📩 Dispatching email to `recipient_email@example.com` via 127.0.0.1:1025...
  [00:00:00 INF] Email has been sent successfully.
```

```s
  encrypt-message <message> <key>  Encrypts a message.

  > dotnet run -- encrypt-message "Foo!" "email@example.com"
  > dotnet run -- encrypt-message --message "Foo!" --key "email@example.com"
  > dotnet run -- encrypt-message -m "Foo!" -k "email@example.com"

  ** Output:
  [00:00:00 INF] Message: GTMDAql3cgoOoeL/1qYvDNzIaQrMWxC4Fg8QAQ1wdKgyfXPNGi4PzfCmOuNR8I4ixLW99Du745Q
  cn6FSbQnpZsFAg8vC+I+Dr9sVV9waS4gRnW+sIliuNRse77tUB6SzYPjZnbJDx4cXcxEcOSz4e8xxnGa7xiA98/rp71RNEQE1Wu
  MYQgrTXyl1qRWPse++zvyWaIqj39p4IiJcfm1a4SuMYvoGGkvu4dupTCcYQrvAbxOUqdccJvg4yOYx0S5HhcuRxzN6EUYkGTSsy
  0uS33eAwMSEOhI99fsj4LshxMius7fZA9Fm5We5rjdhtTwWxwLEzkfqYCZh7jE/YHxGVQ==
```

```s
  decrypt-message <message> <key>  Decrypts a message.

  > dotnet run -- decrypt-message "GTMDAql3cgoOoeL/1qYvDNzIaQrM/..."
  > dotnet run -- decrypt-message --message "GTMDAql3cgoOoeL/1qYvDNzIaQrM/..." --key "email@example.com"

  ** Output:
  [00:00:00 INF] Message: Foo!
```

```s
  send-message <message> <email>

  > dotnet run -- send-message "Foo!" "recipient_email@example.com"
  > dotnet run -- send-message --message "Foo!" --email "recipient_email@example.com"

  ** Output:
  [00:00:00 INF] 📩 Dispatching email to `recipient_email@example.com` via 127.0.0.1:1025...
  [00:00:00 INF] Email has been sent successfully.
```
