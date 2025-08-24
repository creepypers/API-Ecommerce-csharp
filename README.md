# 🛒 ECOM_APIREST - Plateforme E-commerce Microservices

## 📋 Description

ECOM_APIREST est une plateforme e-commerce moderne construite avec une architecture microservices utilisant .NET 9.0. Le projet implémente une solution complète de commerce électronique avec une séparation claire des responsabilités entre les différents services.

## 🏗️ Architecture

Le projet suit une architecture microservices avec les composants suivants :

### 🔐 **ECOM_AuthentificationMicroservice**
- Gestion de l'authentification et autorisation
- JWT Bearer tokens
- Base de données SQL Server avec Entity Framework Core
- API REST sécurisée

### 👥 **ECOM_UtilisateurMicroservice**
- Gestion des profils utilisateurs
- CRUD des informations client
- Intégration avec le service d'authentification

### 📦 **ECOM_ProductsMicroservice**
- Catalogue de produits
- Gestion des catégories
- Recherche et filtrage des produits
- Gestion des stocks

### 🛍️ **ECOM_PanierMicroservice**
- Gestion du panier d'achat
- Ajout/suppression d'articles
- Calcul des totaux
- Persistance des paniers

### 💳 **ECOM_PayementMicroservice**
- Traitement des paiements
- Intégration avec des passerelles de paiement
- Gestion des transactions
- Sécurisation des données de paiement

### 📋 **ECOM_CommandesMicroservice**
- Gestion des commandes
- Suivi des statuts
- Historique des achats
- Intégration avec le panier et les paiements

### 🌐 **ECOM_Gateway**
- API Gateway centralisée avec Ocelot
- Routage intelligent entre microservices
- Documentation Swagger unifiée
- Gestion des CORS et authentification

## 🚀 Technologies Utilisées

- **.NET 9.0** - Framework principal
- **ASP.NET Core** - API Web
- **Entity Framework Core** - ORM pour la persistance
- **SQL Server** - Base de données
- **Ocelot** - API Gateway
- **JWT Bearer** - Authentification
- **Swagger/OpenAPI** - Documentation des API
- **MMLib.SwaggerForOcelot** - Intégration Swagger avec Ocelot

## 📁 Structure du Projet

```
ECOM_APIREST/
├── ECOM_AuthentificationMicroservice/
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   └── Program.cs
├── ECOM_UtilisateurMicroservice/
├── ECOM_ProductsMicroservice/
├── ECOM_PanierMicroservice/
├── ECOM_PayementMicroservice/
├── ECOM_CommandesMicroservice/
├── ECOM_Gateway/
│   ├── Routes/
│   └── ocelot.json
└── ECOM_APIREST.sln
```

## ⚙️ Prérequis

- **.NET 9.0 SDK** ou plus récent
- **Visual Studio 2022** ou **Visual Studio Code**
- **SQL Server** (Express ou Developer Edition)
- **Git** pour la gestion de version

## 🔧 Installation et Configuration

### 1. Cloner le Repository
```bash
git clone [URL_DU_REPO]
cd ECOM_APIREST
```

### 2. Restaurer les Packages NuGet
```bash
dotnet restore
```

### 3. Configuration des Bases de Données
- Créer une base de données SQL Server pour chaque microservice
- Mettre à jour les chaînes de connexion dans `appsettings.json` de chaque service

### 4. Configuration de l'API Gateway
- Vérifier le fichier `ocelot.json` dans le projet Gateway
- Ajuster les routes selon vos besoins

### 5. Lancer les Services
```bash
# Lancer tous les services depuis la solution
dotnet run --project ECOM_APIREST.sln

# Ou lancer individuellement chaque microservice
dotnet run --project ECOM_AuthentificationMicroservice
dotnet run --project ECOM_Gateway
# ... etc pour chaque service
```

## 🌐 Points d'Entrée des API

### API Gateway
- **URL principale** : `https://localhost:7000` (ou port configuré)
- **Documentation Swagger** : `/swagger`

### Microservices (ports par défaut)
- **Authentification** : `https://localhost:7001`
- **Utilisateurs** : `https://localhost:7002`
- **Produits** : `https://localhost:7003`
- **Panier** : `https://localhost:7004`
- **Paiements** : `https://localhost:7005`
- **Commandes** : `https://localhost:7006`

## 🔐 Authentification

Le système utilise JWT Bearer tokens pour l'authentification :

1. **S'inscrire** via le service d'authentification
2. **Se connecter** pour obtenir un token JWT
3. **Utiliser le token** dans l'en-tête `Authorization: Bearer {token}`

## 📚 Documentation

- **Rapport détaillé** : `RapportECOMAPI.pdf`
- **Démonstration vidéo** : `ECOMAPI.mp4`
- **Documentation API** : Accessible via Swagger sur chaque service

## 🧪 Tests

```bash
# Exécuter les tests unitaires
dotnet test

# Exécuter les tests d'intégration
dotnet test --filter Category=Integration
```

## 🚀 Déploiement

### Développement
```bash
dotnet run --environment Development
```

### Production
```bash
dotnet publish -c Release
dotnet run --environment Production
```

## 🔍 Monitoring et Logs

- **Logs** : Configuration dans `appsettings.json` de chaque service
- **Métriques** : Intégration possible avec Application Insights
- **Santé des services** : Endpoints `/health` sur chaque microservice

## 🤝 Contribution

1. Fork le projet
2. Créer une branche feature (`git checkout -b feature/AmazingFeature`)
3. Commit les changements (`git commit -m 'Add some AmazingFeature'`)
4. Push vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrir une Pull Request


---

**Note** : Ce projet est en développement actif. Certaines fonctionnalités peuvent être en cours d'implémentation.
