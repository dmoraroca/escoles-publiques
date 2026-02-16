# Manuel utilisateur (FR)

## 1. Introduction
Ce manuel explique comment utiliser l'application web « Escoles Publiques ».

Public visé :
- administrateurs (`ADM`)
- utilisateurs finaux (`USER`) (accès limité)

Prérequis :
- navigateur moderne
- identifiants valides

## 1.1 Protection des données (important)
- N'incluez pas de données personnelles réelles (noms/e-mails/ID) dans la documentation.
- Pour les captures, utilisez des données de test et masquez/pixellisez toute information sensible avant partage.
- Ce dépôt ne doit pas contenir de captures avec des données réelles.

## 2. Connexion
1. Ouvrez la page de connexion.
2. Saisissez e-mail et mot de passe.
3. Cliquez sur « Connexion ».

Captures :
![Connexion (FR)](../codex_images_real/en/login-admin.png)

## 3. Navigation
Menu principal administrateur :
- Accueil, Écoles, Élèves, Inscriptions, Frais annuels

Sélecteur de langue :
- choisissez une langue dans la barre supérieure (persistée via cookie)

## 4. Écoles (ADM)
- liste/recherche/tri
- créer/modifier/supprimer
- favoris et attribution de périmètre (scope)

## 5. Élèves (ADM)
- créer/modifier/supprimer
- réutilisation d'utilisateurs par e-mail

## 6. Inscriptions (ADM)
- créer/modifier/supprimer une inscription (année scolaire, cours, statut)

## 7. Frais annuels (ADM)
Création :
1. Allez dans Frais annuels.
2. Cliquez sur « Nouveau » (ouvre une fenêtre modale).
3. Sélectionnez l'inscription, saisissez le montant et la date d'échéance.
4. Acceptez la case confidentialité et enregistrez.

Décimales :
- le montant accepte `1000,25` et `1000.25`

Captures :
![Frais annuels : liste (FR)](../codex_images_real/en/quotes-anuals.png)
![Frais annuels : création (modale) (FR)](../codex_images_real/en/quotes-anuals-crear.png)
![Frais annuels : modification (FR)](../codex_images_real/en/quotes-anuals-edit.png)

## Annexe : index des captures (ordre)
1. `docs/codex_images_real/en/login-admin.png`
2. `docs/codex_images_real/en/inici-top.png`
3. `docs/codex_images_real/en/inici-down.png`
4. `docs/codex_images_real/en/escoles.png`
5. `docs/codex_images_real/en/escoles-crear.png`
6. `docs/codex_images_real/en/escoles-detall.png`
7. `docs/codex_images_real/en/escoles-edit.png`
8. `docs/codex_images_real/en/estudiants.png`
9. `docs/codex_images_real/en/inscripcions.png`
10. `docs/codex_images_real/en/quotes-anuals.png`
11. `docs/codex_images_real/en/quotes-anuals-crear.png`
12. `docs/codex_images_real/en/quotes-anuals-edit.png`

## 8. FAQ
- Non autorisé : session expirée, reconnectez-vous
- Champs obligatoires manquants : complétez les entrées requises
