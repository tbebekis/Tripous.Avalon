# Sec And Passwords

`Sec` and `Passwords` are small Tripous Core helpers for common security-related tasks.

- `Sec` provides salt creation, PBKDF2 key derivation, password hashing, password verification, and AES-GCM encryption.
- `Passwords` provides random password generation and password rule validation.

They are helpers, not a complete identity or security framework.

## Password Hashing

Passwords should not be stored as plain text.
The usual Tripous pattern is:

- Create a random salt.
- Hash the password with PBKDF2.
- Store the hash, salt, and iteration count.

```csharp
string Salt = Sec.CreateSalt();
string PasswordHash = Sec.HashPassword(PlainTextPassword, Salt, 100_000);
```

Sample applications store the hash and salt in the user row.

```csharp
CurrentRow["Password"] = PasswordHash;
CurrentRow["Salt"] = Salt;
```

The plain text password is used only at the point where the hash is created.

## Password Verification

To verify a password, load the stored hash and salt and call `VerifyPassword()`.

```csharp
string PasswordHash = User.Properties["Password"] as string;
string Salt = User.Properties["Salt"] as string;

bool IsValid = Sec.VerifyPassword(PlainTextPassword, PasswordHash, Salt, 100_000);
```

`VerifyPassword()` hashes the supplied plain text password again and compares it with the stored hash using a fixed-time comparison.

This is the pattern used by login and password-change code in the sample applications.

## Changing A Password

When changing a password, first verify the current password.
Then create a new salt and hash for the new password.

```csharp
bool IsValid = Sec.VerifyPassword(CurrentPlainTextPassword, PasswordHash, Salt, 100_000);

if (!IsValid)
    throw new TripousException("Current password is invalid.");

string NewSalt = Sec.CreateSalt();
string NewPasswordHash = Sec.HashPassword(NewPlainTextPassword, NewSalt, 100_000);
```

Do not reuse the old salt when setting a new password.

## Key Derivation

`DeriveKey()` derives a 32-byte key from a password and salt using PBKDF2 with SHA-256.

```csharp
byte[] Key = Sec.DeriveKey(PasswordPlainText, Salt, 100_000);
```

This is used internally by `HashPassword()`, `Encrypt()`, and `Decrypt()`.
Application code usually calls the higher-level methods.

## Encryption And Decryption

`Encrypt()` encrypts UTF-8 text with AES-GCM.
It derives the encryption key from a password, salt, and iteration count.

```csharp
string Salt = Sec.CreateSalt();

string CipherText = Sec.Encrypt(
    "Secret value",
    MasterPassword,
    Salt,
    100_000);
```

`Decrypt()` reverses the operation.

```csharp
string PlainText = Sec.Decrypt(
    CipherText,
    MasterPassword,
    Salt,
    100_000);
```

The encrypted payload contains the nonce, authentication tag, and cipher bytes, encoded as Base64.
The salt and iteration count are not embedded in the payload, so the application must store them separately.

## Password Generation

`Passwords.GeneratePassword()` creates a random password using `RandomNumberGenerator`.
It guarantees that every enabled character category appears at least once.

```csharp
string Password = Passwords.GeneratePassword(
    MinLength: 16,
    UseUpperCase: true,
    UseLowerCase: true,
    UseNumbers: true,
    UseSpecialChars: true);
```

If the requested length is smaller than the number of enabled categories, the method increases the length enough to include all required categories.

## Password Validation

`Passwords.IsValid()` checks a password against length and character-category rules.

```csharp
bool IsValid = Passwords.IsValid(
    Password,
    MinLength: 8,
    MaxLength: 32,
    UseLowerChars: true,
    UseUpperChars: true,
    UseDigitChars: true,
    UseSpecialChars: true);
```

The special character set is:

```text
!@#$%^*()-_=+[]{}|?
```

The set intentionally excludes characters such as `&` and `<`, which are often inconvenient in markup or shell contexts.

## Practical Rules

Use these helpers with a few simple rules:

- Store password hash and salt, not plain text passwords.
- Store the iteration count with the hash, or keep it as a versioned application setting.
- Use a new salt whenever a password is created or changed.
- Use the same salt and iteration count only when verifying an existing hash.
- Keep encryption salt and iteration count available for later decryption.
- Do not log plain text passwords, hashes, salts, or decrypted secrets.

## When To Use Them

Use `Sec` for local application password hashing and small encrypted values.
Use `Passwords` for password generation and simple password rule validation.

For protocols, token systems, federated login, certificate handling, or multi-tenant security policy, use a dedicated security library or platform service.
