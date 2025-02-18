# Postbox
![](https://img.shields.io/badge/Built_with-.NET_8.0-blue)

📫 Postbox is a lightweight encryption tool which allows users to generate key pairs, exchange public keys, encrypt and decrypt messages, and communicate securely over SMTP using [RSA](https://en.wikipedia.org/wiki/RSA_(cryptosystem)).

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
