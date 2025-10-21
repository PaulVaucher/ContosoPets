# ContosoPets - Application de Gestion d'Animaux

## 📋 Description

ContosoPets est une application console .NET développée dans le cadre d'un exercice de formation Microsoft Learn en collaboration avec mon mentor Matthias Morard. Ce projet met en pratique les principes de la **Programmation Orientée Objet (OOP)** et de l'**Architecture Propre (Clean Architecture)**.

L'application permet de gérer une base de données d'animaux domestiques (chiens et chats) avec leurs informations détaillées telles que l'âge, la description physique, la personnalité et les surnoms.

> **Projet éducatif** basé sur l'exercice Microsoft Learn : [Challenge project - branching and looping](https://github.com/MicrosoftLearning/Challenge-project-branching-looping-CSharp/)

## 🚀 Fonctionnalités

- ✅ **Affichage** de tous les animaux enregistrés
- ✅ **Ajout** de nouveaux animaux (limité à 8 animaux)
- ✅ **Mise à jour** des informations incomplètes (âge, description physique)
- ✅ **Gestion** des surnoms et descriptions de personnalité
- ✅ **Modification** individuelle de l'âge et de la personnalité
- ✅ **Recherche** d'animaux par caractéristiques spécifiques
- ✅ **Interface console** interactive avec menu
- ✅ **Persistance des données** avec PostgreSQL et NHibernate

## 🏗️ Architecture

Le projet suit les principes de la **Clean Architecture** avec une séparation claire des responsabilités :

```ContosoPets/
├── Domain/                # Logique métier et entités
│   ├── Entities/          # Entités (Animal, Dog, Cat)
│   ├── Services/          # Services de domaine
│   ├── ValueObjects/      # Objets valeur (AnimalId)
│   ├── Builders/          # Pattern Builder pour création d'objets
│   └── Constants/         # Constantes de l'application
├── Application/           # Logique applicative
│   ├── Services/          # Services applicatifs (Facade)
│   ├── UseCases/          # Requests/Results (CQRS-like)
│   ├── Ports/             # Interfaces (Repository, ILinePrinter)
│   └── SharedKernel/      # Interfaces partagées
├── Infrastructure/        # Accès aux données et technologies
│   ├── Repositories/      # Implémentation des repositories
│   ├── Database/          # Configuration NHibernate + PostgreSQL
│   ├── Entities/          # Entités NHibernate
│   └── Output/            # Implémentation console
├── Presentation/          # Interface utilisateur
│   └── ConsoleApp/        # Application console avec pattern Command
├── Scripts/               # Scripts utilitaires
│   └── build/             # Scripts de build et nettoyage
└── ContosoPets.UnitTests/ # Tests unitaires
```

## 🛠️ Technologies Utilisées

- **Framework** : .NET 8
- **Langage** : C# 12.0
- **Base de données** : PostgreSQL
- **ORM** : NHibernate
- **Tests** : xUnit, FluentAssertions
- **Injection de dépendances** : Microsoft.Extensions.DependencyInjection
- **Logging** : Microsoft.Extensions.Logging

## ⚙️ Configuration

Le projet utilise l'injection de dépendances native de .NET avec une configuration modulaire :

- **ServiceContainer** : Point d'entrée de la configuration
- **ServiceCollectionExtensions** : Extensions pour l'infrastructure
- **Configuration par couches** : Domain, Application, Infrastructure, Presentation

## 📦 Installation et Exécution

### Prérequis
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (version 12 ou supérieure)

### Étapes d'installation

1. **Cloner le repository**
	git clone https://github.com/PaulVaucher/ContosoPets.git 
    cd ContosoPets

2. **Configurer PostgreSQL**
   - Créer une base de données `contosopets`
   - Les chaînes de connexion sont configurées dans les fichiers de configuration

3. **Restaurer les packages NuGet**
	dotnet restore

4. **Compiler le projet**
	dotnet build

5. **Exécuter l'application**
	cd Presentation/ConsoleApp dotnet run


## 🎮 Utilisation

Au lancement, l'application présente un menu interactif avec les options suivantes :
	
	Welcome to the Contoso PetFriends app. Your main menu options are:

	1.	List all of our current pet information
	2.	Add a new animal friend to the application
	3.	Ensure animal ages and physical descriptions are complete
	4.	Ensure animal nicknames and personality descriptions are complete
	5.	Edit an animal's age
	6.	Edit an animal's personality description
	7.	Display all cats with a specified characteristic
	8.	Display all dogs with a specified characteristic
	0.	Exit the application


### Exemples d'utilisation

- **Ajouter un animal** : Choisir l'option 2, spécifier "dog" ou "cat", puis remplir les informations
- **Rechercher par caractéristique** : Options 7 ou 8, saisir une caractéristique (ex: "playful", "brown", "large")
- **Compléter les informations** : Options 3 ou 4 pour mettre à jour les champs marqués comme "tbd" ou "?"

## 🧪 Tests

Exécuter les tests unitaires :
	dotnet test

Les tests couvrent :
- **Services applicatifs** (`AnimalApplicationService`)
- **Services de domaine** (`AnimalDomainService`)
- **Builders** (`AnimalBuilder`)
- **Entités** (`Animal`, `Dog`, `Cat`)
- **Configuration** (`ServiceContainer`)
- **Scénarios d'utilisation** complets

### Structure des tests
```ContosoPets.UnitTests/
├── Application/Services/   # Tests des services applicatifs
├── Domain/
│   ├── Builders/           # Tests du pattern Builder
│   ├── Entities/           # Tests des entités métier
│   └── Services/           # Tests des services de domaine
├── Presentation/           # Tests de configuration et runners
│   └── UI/                 # Tests du MenuHandler
├── TestInfrastructure/     # Infrastructure de test
│   └── Fakes/              # Implémentations factices
└── Helpers/                # Utilitaires de test (TestDataBuilder)
```


## 🔧 Scripts de Build

Le projet inclut un script PowerShell de nettoyage post-build :

	- Nettoyage standard (automatique à chaque build)
		.\Scripts\build\post-build-cleanup.ps1

	- Nettoyage approfondi (manuel)
		.\Scripts\build\post-build-cleanup.ps1 -DeepClean


**Fonctionnalités du script** :
- Suppression des fichiers temporaires (`.tmp`, `.log`, `.cache`)
- Nettoyage des dossiers temporaires spécifiques
- Mode DeepClean avec optimisation du Garbage Collector
- Amélioration de la gestion mémoire durant le développement

## ⚠️ Notes de Sécurité

> **Important** : Les informations de connexion à la base de données PostgreSQL sont stockées en clair dans les fichiers de configuration. Cette pratique est acceptable uniquement pour ce projet de formation qui n'est **pas destiné à être déployé en production**.

## 🎯 Objectifs Pédagogiques

Ce projet illustre les concepts suivants :

### Architecture et Design Patterns
- **Clean Architecture** et séparation des responsabilités
- **Domain-Driven Design (DDD)** avec services de domaine
- **Pattern Repository** pour l'accès aux données
- **Pattern Builder** pour la création d'objets complexes
- **Pattern Command** pour les actions utilisateur

### Patterns Implémentés
- **Clean Architecture** avec séparation stricte des couches
- **Repository Pattern** pour l'abstraction des données
- **Builder Pattern** pour la création d'entités complexes
- **Command Pattern** pour les actions utilisateur (menu)
- **Facade Pattern** dans les services applicatifs
- **Strategy Pattern** pour les différents types d'animaux
- **CQRS-like** avec Request/Result dans les UseCases

### Techniques .NET
- **Inversion de dépendance** et injection de dépendances
- **ORM NHibernate** avec mapping objet-relationnel
- **Logging structuré** avec Microsoft.Extensions.Logging
- **Gestion des transactions** et rollback automatique

### Bonnes Pratiques
- **Tests unitaires** avec couverture complète
- **Validation métier** centralisée dans le domaine
- **Gestion d'erreurs** robuste avec try-catch appropriés
- **Configuration** externalisée et flexible

## 📚 Ressources d'Apprentissage

- **Exercice original** : [Microsoft Learn - Challenge project branching looping C#](https://github.com/MicrosoftLearning/Challenge-project-branching-looping-CSharp/)
- **Clean Architecture** : Concepts de Robert C. Martin
- **Domain-Driven Design** : Approche d'Eric Evans
- **Documentation .NET 8** : [docs.microsoft.com](https://docs.microsoft.com/dotnet/)

## 👥 Contributeurs

- **[Paul Vaucher](https://github.com/PaulVaucher)** - Développeur principal
- **[Matthias Morard]** - Supervision et guidance architecturale

## 📄 Licence

Ce projet est à des fins éducatives et n'a pas de licence spécifique.

---

*Projet réalisé dans le cadre d'un exercice Microsoft Learn pour l'apprentissage des bonnes pratiques de développement .NET et de l'architecture logicielle*