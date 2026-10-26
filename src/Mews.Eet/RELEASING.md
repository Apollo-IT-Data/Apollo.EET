# Vydání balíčku Apollo.EET

Balíček `Apollo.EET` se publikuje do GitHub Packages organizace `Apollo-IT-Data` automaticky po pushnutí tagu `v*` (např. `v2.0.1`). Ručně se nic nesestavuje.

## Jak to funguje

Balíček nesestavuje Visual Studio ani lokální počítač, ale GitHub Actions podle pravidel v souboru [.github/workflows/publish.yml](../../.github/workflows/publish.yml). GitHub tento soubor čte při každém pushi a spustí ho, když push odpovídá podmínce v sekci `on:`:

```yaml
on:
  push:
    tags:
      - "v*"      # spusť se jen při pushi tagu začínajícího na "v"
```

Běžný push commitů tedy nic nespustí, workflow se rozjede jen při pushi tagu jako `v2.0.0`.

Celý průběh:

```
vývojář: git push origin v2.0.0
           │
GitHub:  "přišel tag v* → spustím Publish Apollo.EET"
           │
           ▼  na dočasném serveru s Ubuntu (runs-on: ubuntu-latest)
         1. checkout              stáhne kód přesně z commitu, na který tag ukazuje
         2. setup-dotnet          nainstaluje .NET SDK 10
         3. VERSION=2.0.0         vezme název tagu a odřízne „v“
         4. dotnet pack           sestaví Apollo.EET.2.0.0.nupkg
         5. dotnet nuget push     nahraje ho do GitHub Packages organizace
           │
           ▼
         github.com/orgs/Apollo-IT-Data/packages → Apollo.EET 2.0.0
```

K nahrání workflow nepotřebuje žádné heslo ani PAT. GitHub mu na dobu běhu přidělí dočasný `GITHUB_TOKEN` a řádek `permissions: packages: write` mu dovolí publikovat. Kroky běhu a jejich výstup najdete v záložce Actions repozitáře.

**Proč tag, a ne verze v souboru:** tag je pojmenovaná značka na konkrétním commitu. Verze se tak určuje jedním příkazem a u každé verze balíčku je vždy jasné, ze kterého kódu vznikla. Proto `Mews.Eet.csproj` neobsahuje pevnou `<Version>`.

## Jak vydat novou verzi

1. Upravte kód na větvi `eet2.0`, commitněte a pushněte. Zatím se nic nepublikuje.
2. Vytvořte tag s novým číslem verze na posledním commitu a pushněte ho:
   ```bash
   git tag v2.0.1
   git push origin v2.0.1
   ```
   Ve Visual Studiu: okno Git Repository → pravým na commit → New Tag, pak push včetně tagů.
3. Počkejte, až v záložce Actions doběhne běh „Publish Apollo.EET“. Na GitHubu pak bude `Apollo.EET 2.0.1`.
4. V aplikacích, které balíček používají (`PPAS-EET`, Lexum), zvyšte verzi na 2.0.1.

## Jak volit číslo verze

Číslo má tvar `hlavní.vedlejší.oprava` (semantic versioning):

| Změna | Příklad | Nová verze |
|---|---|---|
| Oprava chyby, API beze změny | oprava formátu XML | `v2.0.1` |
| Nová funkce, starý kód dál funguje | nová volitelná vlastnost | `v2.1.0` |
| Nekompatibilní změna | přejmenovaná nebo odebraná třída či metoda | `v3.0.0` |
| Testovací verze | před ostrým nasazením | `v2.1.0-beta.1` |

## Na co si dát pozor

- Vydanou verzi nejde přepsat ani opravit. Chyba v 2.0.1 znamená vydat 2.0.2.
- Když pushnete tag s verzí, která už existuje, workflow ji kvůli `--skip-duplicate` jen přeskočí a nic nového nevznikne.
- Tag patří až na commit, který je pushnutý a má být ve verzi. Tag na starším commitu sestaví starší kód.
