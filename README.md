# Postbox
![](https://img.shields.io/badge/Built_with-.NET_8.0-blue)

📫 Postbox is a lightweight encryption tool which allows users to generate key pairs, exchange public keys, encrypt and decrypt messages, and communicate securely over SMTP using [RSA](https://en.wikipedia.org/wiki/RSA_(cryptosystem)).

## Configuration

1. Install and configure a local SMTP Server like [Proton Bridge](https://proton.me/mail/bridge)
2. Update the values in `.env.EXAMPLE` and rename to `.env`.

  ```s
  SMTP_SERV=YOUR_SMTP_SERVER
  SMTP_PORT=YOUR_SMTP_PORT
  SMTP_USER=YOUR_EMAIL_ADDRESS
  SMTP_PASS=YOUR_PASSWORD
  ```

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
  generate-keys                        Generates a 2048-bit RSA key pair.
  encrypt-message <--message> <--key>  Encrypts a message.
  decrypt-message <--message> <--key>  Decrypts a message.
```
