# Password Manager

This is the fifth Tripous sample application.

Purpose

- Use SQLite.
- Use one main module.
- Use a lookup table for categories.
- Use SYS_CONFIG for settings and vault metadata.
- Use a service class for encryption.
- Use a DataModule for encrypting and decrypting secret fields.
- Avoid AppUsers.
- Avoid Locators.
- Avoid CodeProviders.
- Avoid plaintext secrets in the database.

Main scenario

- The application starts with a hidden startup window.
- The database and schema are created first.
- If no master password exists, the user must create one.
- If a master password exists, the user must unlock the vault.
- The real MainWindow is shown only after the vault is unlocked.
- Credentials are edited through a normal Tripous DataForm.
- Category is a normal lookup table.

Master password

- The master password is required before the user can enter the application.
- The sample policy requires at least 8 characters.
- The sample policy requires at least one uppercase letter.
- The sample policy requires at least one lowercase letter.
- The sample policy requires at least one digit.
- The developer may harden these rules.
- A real application may require a longer password.
- A real application may require symbols.
- A real application may increase the KDF iteration count.
- The plaintext master password is never stored.
- SYS_CONFIG stores the salt, verifier hash and KDF iteration count.
- SYS_CONFIG does not store the encryption key.

Encryption

- The SQLite database itself is not encrypted.
- Only credential secret fields are encrypted.
- Credential.Password is encrypted before commit.
- Credential.Notes is encrypted before commit.
- Credential.Password is decrypted after edit load.
- Credential.Notes is decrypted after edit load.
- VaultService keeps the runtime key only in memory.
- VaultService.Lock() clears the runtime key.
- This sample uses PBKDF2 with SHA-256.
- This sample uses AES-GCM for field encryption.
- Encrypted field values use a small v1 prefix so the DataModule can avoid double encryption.

Lock

- Lock does not delete the database.
- Lock does not change the master password.
- Lock does not encrypt or decrypt existing rows.
- Lock clears the runtime encryption key from memory.
- Lock sets the runtime key reference to null.
- After Lock the vault is considered locked.
- After Lock the application cannot encrypt or decrypt credential fields.
- After Lock the user must unlock the vault again with the master password.
- VaultService.Lock() uses CryptographicOperations.ZeroMemory() before forgetting the key.

Tables

- SYS_CONFIG stores configuration values and vault metadata.
- SYS_LOG stores application log rows.
- Category stores credential category rows.
- Credential stores structured credential rows.

Credential fields

- Id is the primary key.
- CategoryId is a lookup field.
- Title is the display name.
- UserName stores the login name.
- Url stores the related address.
- Password stores encrypted password text.
- Notes stores encrypted notes text.
- CreatedAt is read-only in the UI.
- UpdatedAt is read-only in the UI.

List filters

- Title contains.
- UserName contains.
- Url contains.
- Category contains.
- UpdatedAt between.

Config

- PasswordManager.MasterSalt stores the master password salt.
- PasswordManager.MasterHash stores a verifier hash, not the encryption key.
- PasswordManager.KdfIterations stores the PBKDF2 iteration count.
- PasswordManager.MinimumPasswordLength stores the sample minimum length.
- PasswordManager.AutoOpenCredentialList controls startup form opening.

Services

- VaultService owns password validation, key derivation, encryption and decryption.
- CredentialTransferService owns the import/export sample.
- Services keep application logic outside forms.
- DataModule code calls services but does not show UI.

DataModule

- CredentialDataModule sets default values.
- CredentialDataModule decrypts secrets after Edit().
- CredentialDataModule encrypts secrets before Commit().
- CredentialDataModule restores plaintext in memory after Commit().
- The DataModule does not ask questions.
- The DataModule does not show dialogs.

Import/export

- Export writes encrypted rows to credential-export.json in the application folder.
- Import reads encrypted rows from the same file.
- The sample export is intentionally encrypted.
- The sample does not produce a plaintext password export.
- A real application should ask the user for export destination and backup policy.

Startup sequence

- App creates HiddenMainWindow.
- HiddenMainWindow becomes Avalonia desktop MainWindow.
- HiddenMainWindow.Opened calls AppHost.Start().
- AppHost initializes SysConfig.
- AppHost loads or creates DbConnections.json.
- AppHost creates the SQLite database when needed.
- Registry.RegisterSchemas() registers SchemaVersion1.
- Schemas.Execute() creates the database tables.
- AppHost creates the default SqlStore.
- Registry.RegisterDescriptors() registers lookups, modules, forms and config properties.
- AppHost creates or verifies the master password.
- AppHost registers commands.
- AppHost creates and shows MainWindow.

Files

- PasswordManager.csproj
- Program.cs
- App.axaml
- App.axaml.cs
- HiddenMainWindow.cs
- MainWindow.axaml
- MainWindow.axaml.cs
- CreateMasterPasswordDialog.axaml
- CreateMasterPasswordDialog.axaml.cs
- UnlockVaultDialog.axaml
- UnlockVaultDialog.axaml.cs
- AppHost/AppHost.cs
- AppHost/AppHost.Startup.cs
- AppHost/AppHost.Commands.cs
- AppHost/AppHost.Ui.cs
- Services/VaultService.cs
- Services/CredentialTransferService.cs
- Services/CredentialTransferRow.cs
- Data/DataModules/CredentialDataModule.cs
- Data/Registry/SchemaVersion1.cs
- Data/Registry/RegistryVersion.cs
- Data/Registry/RegistryVersion1.cs
- Data/Registry/Registry.cs
- ReadMe.txt
