# MyCrypt &middot; [![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](./LICENSE) ![.NET](https://img.shields.io/badge/.NET-512BD4?logo=dotnet&logoColor=white) ![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)  ![Static Badge](https://img.shields.io/badge/CLI-8A2BE2)

MyCrypt is a command-line interface (CLI) tool built with .NET for encrypting, decrypting, and validating files in a simple, secure, and efficient way.

## Quick start
1. Clone the repo: `git clone https://github.com/Adler-Targino/MyCrypt.git`
2. From the terminal at the root of the project either build using `dotnet build` or run with `dotnet run`
3. Go to [Usage](https://github.com/Adler-Targino/MyCrypt#usage) and learn more about the project.

*(An installer with quick setup will be provided later, once the project has most of it's functionalities implemented, for now, setting up the aplication must be done manually)*

## Features
- File encryption
- File decryption
- Cryptographic Key Exporting
- File integrity validation

## Technologies used
- Built with .NET
- Supports AES Encryption/Decryption (More to come)
- SHA Hashing & Hmac validation
  
## Usage
Once you properly set up the aplication, you will be able to perform the following operations:
### File encryption
File encryption can be done as easily as running this command on your terminal.

```
mycrypt encrypt .\file.txt
```
Once ran, a random 32 bytes (44 characters) long, random key is displayed at the terminal and your file is encrypted using it.

**Options:**

- `-a|--algorithm` Defines the encryption algorithm to be used. Default: AES
- `-c|--compression` Defines the compression algorithm to be used. Default: None
- `-k|--key` Defines a key which will be used for encryption, it can be a text key or a file containing the key. (If not present, a random key is created to encrypt your file)
- `-o|--output` Defines the output path of the encrypted file. (By default the output is in the same folder as the input file)
- `-m|--mac` Defines the message authentication code algorithm (HMACSHA256 | None). Default: HMACSHA256
- `-d|--delete` Deletes the original file after encryption.

**Example**
```
mycrypt encrypt .\file.txt --key my-secret-key --output top-secret -d
```
Congratulations! Now your `file.txt` is encrypted into the file `top-secret.myc` and can only be decrypted with the key `my-secret-key`. And there are no traces left of your original file.

<sub>*I can only wonder what secrets you were trying to hide...*</sub>

### File decryption
Files encrypted with MyCrypt can be decrypted using the same key used for encryption. (So don't lose your key if you want your files back)

Assuming we are using the same file from the previous example, decryption can be done by running the following command.

```
mycrypt decrypt .\top-secret.myc my-secret-key
```
As simple as that you have your previously encrypted file back.

**Options:**

- `-o|--output` Defines the output path of the decrypted file. (By default the output is in the same folder as the input file)
- `-d|--delete` Deletes the original file after decrypted.

**Example**
```
mycrypt decrypt .\top-secret.myc my-secret-key --output file -d
```
Running the above command decrypts your `top-secret.myc` file back into `file.txt` while deleting the encrypted file.

### Key Generation
The encrypt command already has built in key generation when you don't specify a key to be used. 
But in some cases you just want to create a random key before starting to encrypt your files.

In that case you can use the following command.
```
mycrypt key generate
```

**Options:**

- `-t|--type` Type of the key that is going to be generated. Default: AES
- `-e|--export` Exports the key to a `.myk` file in the specified path. 
(The file is accepted as a key for both the encrypt and decrypt commands)

**Example**
```
mycrypt key generate --export my-secret-key
```
A random key will be generated. And exported to the file `my-secret-key.myk`

Note: Differently from the encrypt command where a hash is created to be used 
as a key depending on your input having or not a valid lenght, the `key generate` command
will **_always_** generate a random key, independently of any input.

### File validation
If you are planning on sharing your encrypted files with someone it is a good pratice to validate them to ensure file integrity. For that reason, MyCrypt has built in file validation using HMAC.

But it's also possible to add an extra layer of validation, or even validate non encrypted files before sending them by using the following commands.

**ComputeHash**  
Command that computes the cryptographic hash of a file (By default using SHA384) 
```
mycrypt validation compute-hash top-secret.myc
```
A validation hash for your selected file will be shown. Save that hash for future file validation.

**Verify**  
Using the previously computed hash you can verify your file integritry.
```
mycrypt validation verify top-secret.myc e49ab3a941e81e8917d92bf12913733637815fec1b02a9b92b31c237502a92d0d50e01513bb0af795ea8d98e61a53f1c
```
Validation result is shown telling if the file is valid or not.

### Notes
MyCrypt is built using Spectre.Console.Cli, and has fully a functional help tag for each command and it's arguments or options

---

# What's next?
MyCrypt is a hobby project, so new functionalities might take some time to come. 
But there are a few things already on radar for the next versions, such as:
- [X] Importing/Exporting Keys as files
- [X] File validation on decryption.
- [X] File Compression.
- [ ] New encryption Algorithms
- [ ] New compression Algorithms

## When an official release is coming?
Once the project has all the initial ideas implemented. (Encryption, Validation, Compression). 
And a proper testing flux is added, it won't take long to release. I just want to make sure everything is working as intended before making an official release.

## And finally...
If you've come so far as reading this, thank you for your time!

Feel free to share or contribute with this repo, just try to keep it simple.

