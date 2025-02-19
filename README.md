# Postbox
![](https://img.shields.io/badge/Built_with-.NET_8.0-blue)

📫 Postbox is a lightweight encryption tool which allows users to generate key pairs, exchange public keys, encrypt and decrypt messages, and communicate securely over SMTP using [RSA](https://en.wikipedia.org/wiki/RSA_(cryptosystem)).

## Configuration

1. Install and configure a local SMTP Server like [Proton Bridge](https://proton.me/mail/bridge).
2. Update the values in `.env.EXAMPLE` and rename to `.env`.

  ```s
  SMTP_SERV=YOUR_SMTP_SERVER * Defaults to 127.0.0.1
  SMTP_PORT=YOUR_SMTP_PORT * Defaults to 1025 
  SMTP_USER=YOUR_EMAIL_ADDRESS
  SMTP_PASS=YOUR_PASSWORD
  ```
  The `SMTP_USER` value will be the name of your public and private keys: `YOUR_EMAIL_ADDRESS_public.pem` and `YOUR_EMAIL_ADDRESS_private.pem`. These will then act as the default keys when encrypting and decrypting messages, unless another key file is specified with the `--key` parameter.

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
  generate-keys                        Generates a new key pair.
  encrypt-message <--message> <--key>  Encrypts a message.
  decrypt-message <--message> <--key>  Decrypts a message.
```

### Examples

```s
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
> dotnet run -- decrypt-message "GTMDAql3cgoOoeL/1qYvDNzIaQrM/..."
> dotnet run -- decrypt-message --message "GTMDAql3cgoOoeL/1qYvDNzIaQrM/..." --key "email@example.com"

** Output:
[00:00:00 INF] Message: Foo!
```
