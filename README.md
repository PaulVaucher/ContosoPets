# ContosoPets - Application de Gestion d'Animaux

## 📋 Description

ContosoPets est une application console .NET développée dans le cadre d'un exercice de formation Microsoft Learn en collaboration avec mon mentor Matthias Morard. Ce projet met en pratique les principes de la **Programmation Orientée Objet (OOP)** et de l'**Architecture Propre (Clean Architecture)**.

L'application permet de gérer une base de données d'animaux domestiques (chiens et chats) avec leurs informations détaillées telles que l'âge, la description physique, la personnalité et les surnoms. Les données sont stockées localement dans un fichier JSON pour simplifier l'installation et l'utilisation.

> **Projet éducatif** basé sur l'exercice Microsoft Learn : [Challenge project - branching and looping](https://github.com/MicrosoftLearning/Challenge-project-branching-looping-CSharp/)

## 🌟 Branches du Projet

Ce repository contient deux implémentations distinctes :

- **`main`** _(cette branche)_ : Stockage des données en JSON local avec `System.Text.Json`
- **`ContosoPets-ORM`** : Version avec base de données PostgreSQL et ORM NHibernate

## 🚀 Fonctionnalités

- ✅ **Affichage** de tous les animaux enregistrés
- ✅ **Ajout** de nouveaux animaux (limité à 8 animaux)
- ✅ **Mise à jour** des informations incomplètes (âge, description physique)
- ✅ **Gestion** des surnoms et descriptions de personnalité
- ✅ **Modification** individuelle de l'âge et de la personnalité
- ✅ **Recherche** d'animaux par caractéristiques spécifiques
- ✅ **Interface console** interactive avec menu
- ✅ **Persistance des données** en JSON local
- ✅ **Sérialisation/désérialisation** automatique avec conversion personnalisée

## 🏗️ Architecture

Le projet suit les principes de la **Clean Architecture** avec une séparation claire des responsabilités :

```
ContosoPets/
├── Domain/                 # Logique métier et entités
│   ├── Entities/          # Entités (Animal, Dog, Cat)
│   ├── Services/          # Services de domaine
│   ├── ValueObjects/      # Objets valeur (AnimalId)
│   ├── Builders/          # Pattern Builder pour création d'objets
│   └── Constants/         # Constantes de l'application
├── Application/           # Logique applicative
│   ├── Services/          # Services applicatifs
│   ├── UseCases/         # Cas d'utilisation (Request/Result)
│   ├── Ports/            # Interfaces (Repository, ILinePrinter)
│   └── SharedKernel/     # Interfaces partagées
├── Infrastructure/        # Accès aux données et technologies
│   ├── Repositories/     # Implémentation repository JSON
│   ├── Serialization/    # Convertisseurs JSON personnalisés
│   ├── Output/           # Implémentation console
│   └── DI/               # Configuration injection de dépendances
├── Presentation/         # Interface utilisateur
│   └── ConsoleApp/       # Application console avec pattern Command
├── Scripts/              # Scripts utilitaires
│   └── build/           # Scripts de build et nettoyage
├── Resources/            # Fichiers de données
│   └── animals.json     # Stockage JSON des animaux
└── ContosoPets.UnitTests/ # Tests unitaires
```

## 🛠️ Technologies Utilisées

- **Framework** : .NET 8
- **Langage** : C# 12.0
- **Stockage de données** : JSON local (`System.Text.Json`)
- **Sérialisation** : Convertisseurs JSON personnalisés
- **Tests** : xUnit, FluentAssertions
- **Injection de dépendances** : Microsoft.Extensions.DependencyInjection
- **Scanning d'assemblages** : Scrutor pour l'auto-registration

## 📦 Installation et Exécution

### Prérequis
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Étapes d'installation

1. **Cloner le repository**
```
git clone https://github.com/PaulVaucher/ContosoPets.git
cd ContosoPets
```

2. **Restaurer les packages NuGet**
```
dotnet restore
```

3. **Compiler le projet**
```
dotnet build
```

4. **Exécuter l'application**
```
cd Presentation/ConsoleApp
dotnet run
```

### 📁 Stockage des données

Les données sont automatiquement sauvegardées dans :
```
Resources/animals.json
```

Le fichier est créé automatiquement au premier lancement de l'application.

## 🎮 Utilisation

Au lancement, l'application présente un menu interactif avec les options suivantes :

```
Welcome to the Contoso PetFriends app. Your main menu options are:

1. List all of our current pet information
2. Add a new animal friend to the application  
3. Ensure animal ages and physical descriptions are complete
4. Ensure animal nicknames and personality descriptions are complete
5. Edit an animal's age
6. Edit an animal's personality description
7. Display all cats with a specified characteristic
8. Display all dogs with a specified characteristic
0. Exit the application
```

### Exemples d'utilisation

- **Ajouter un animal** : Choisir l'option 2, spécifier "dog" ou "cat", puis remplir les informations
- **Rechercher par caractéristique** : Options 7 ou 8, saisir une caractéristique (ex: "playful", "brown", "large")
- **Compléter les informations** : Options 3 ou 4 pour mettre à jour les champs marqués comme "tbd" ou "?"

### 💾 Persistance automatique

- **Sauvegarde automatique** : Toutes les modifications sont immédiatement sauvegardées
- **Chargement au démarrage** : Les données sont automatiquement chargées depuis le fichier JSON
- **Gestion d'erreurs** : Récupération gracieuse en cas de fichier corrompu

## 🧪 Tests

Exécuter les tests unitaires :

```
dotnet test
```

Les tests couvrent :
- **Services applicatifs** (`AnimalApplicationService`)
- **Services de domaine** (`AnimalDomainService`)
- **Builders** (`AnimalBuilder`)
- **Entités** (`Animal`, `Dog`, `Cat`)
- **Configuration** (`ServiceContainer`)
- **Scénarios d'utilisation** complets

### Structure des tests
```
ContosoPets.UnitTests/
├── Application/Services/    # Tests des services applicatifs
├── Domain/
│   ├── Builders/           # Tests du pattern Builder
│   ├── Entities/           # Tests des entités métier
│   └── Services/           # Tests des services de domaine
├── Presentation/           # Tests de configuration et runners
│   ├── UI/                 # Tests du MenuHandler
│   └── Configuration/      # Tests de ServiceContainer
├── TestInfrastructure/     # Infrastructure de test
│   └── Fakes/             # Implémentations factices
└── Helpers/               # Utilitaires de test (TestDataBuilder)
```

## 🔧 Scripts de Build

Le projet inclut un script PowerShell de nettoyage post-build :

```
# Nettoyage standard (automatique à chaque build)
.\Scripts\build\post-build-cleanup.ps1

# Nettoyage approfondi (manuel)
.\Scripts\build\post-build-cleanup.ps1 -DeepClean
```

**Fonctionnalités du script** :
- Suppression des fichiers temporaires (`.tmp`, `.log`, `.cache`)
- Nettoyage des dossiers temporaires spécifiques
- Mode DeepClean avec optimisation du Garbage Collector
- Amélioration de la gestion mémoire durant le développement

## ⚙️ Configuration

Le projet utilise l'injection de dépendances native de .NET avec une configuration modulaire :

- **ServiceContainer** : Point d'entrée de la configuration simplifiée
- **ServiceCollectionExtensions** : Extensions pour l'infrastructure
- **Auto-registration** : Scanning automatique des services avec Scrutor
- **Configuration sans base de données** : Aucune chaîne de connexion requise

## 🎯 Objectifs Pédagogiques

Ce projet illustre les concepts suivants :

### Architecture et Design Patterns
- **Clean Architecture** et séparation des responsabilités
- **Pattern Repository** pour l'abstraction des données
- **Pattern Builder** pour la création d'objets complexes
- **Pattern Command** pour les actions utilisateur
- **CQRS-like** avec Request/Result dans les UseCases

### Techniques .NET
- **Injection de dépendances** native Microsoft
- **Sérialisation JSON** avec `System.Text.Json`
- **Convertisseurs personnalisés** pour types complexes
- **Scanning d'assemblages** avec Scrutor
- **Gestion de fichiers** et persistance locale

### Bonnes Pratiques
- **Tests unitaires** avec couverture complète
- **Validation métier** centralisée dans le domaine
- **Gestion d'erreurs** robuste avec try-catch appropriés
- **Configuration** simplifiée sans dépendances externes
- **Sérialisation type-safe** avec gestion des erreurs

## 🔄 Comparaison des Branches

| Aspect | main (JSON) | nhibernate-postgresql |
|--------|-------------|----------------------|
| **Stockage** | Fichier JSON local | Base de données PostgreSQL |
| **ORM** | Aucun | NHibernate |
| **Setup** | Aucun prérequis | Installation PostgreSQL |
| **Persistance** | Fichier système | Base de données relationnelle |
| **Performance** | Rapide pour petits volumes | Optimisé pour gros volumes |
| **Complexité** | Simple | Avancée |

## 📚 Ressources d'Apprentissage

- **Exercice original** : [Microsoft Learn - Challenge project branching looping C#](https://github.com/MicrosoftLearning/Challenge-project-branching-looping-CSharp/)
- **Clean Architecture** : Concepts de Robert C. Martin
- **Domain-Driven Design** : Approche d'Eric Evans
- **System.Text.Json** : [Documentation Microsoft](https://docs.microsoft.com/dotnet/standard/serialization/system-text-json-overview)
- **Documentation .NET 8** : [docs.microsoft.com](https://docs.microsoft.com/dotnet/)

## 👥 Contributeurs

- **[Paul Vaucher](https://github.com/PaulVaucher)** - Développeur principal
- **[Matthias Morard](https://www.linkedin.com/in/matthias-morard-7a7a60177/)** - Supervision et guidance architecturale

## 📄 Licence

Ce projet est à des fins éducatives et n'a pas de licence spécifique.

---

*Projet réalisé dans le cadre d'un exercice Microsoft Learn pour l'apprentissage des bonnes pratiques de développement .NET et de l'architecture logicielle*