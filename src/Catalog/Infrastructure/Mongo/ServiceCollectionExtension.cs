﻿using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Octopus.Catalog.Core.Contract.Products.Services;
using Octopus.Catalog.Core.Domain.Products.Entities;
using Octopus.Catalog.Core.Domain.Products.Services;
using Octopus.Catalog.Core.Domain.Products.ValueObjects;
using Octopus.Catalog.Core.Mongo.Products;
using Octopus.Core.Domain.Entities;

namespace Octopus.Catalog.Core.Mongo;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCatalogMongoServices(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, MongoProductRepository>();
        services.AddSingleton<IProductDataService, MongoProductDataService>();

        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<Product>(MongoProductRepository.CollectionName);
        });


        BsonSerializer.RegisterSerializer(new ProductIdBsonSerializer());

        BsonClassMap.RegisterClassMap<EntityBase<ProductId>>(cm =>
        {
            cm.MapMember(m => m.Id).SetElementName("_id");

            cm.SetIgnoreExtraElements(true);
        });

        BsonClassMap.RegisterClassMap<Product>(cm =>
        {
            cm.MapMember(m => m.Sku).SetElementName("Sku");
            cm.MapMember(m => m.Price).SetElementName("Price");
            cm.MapMember(m => m.Code).SetElementName("Code");
            cm.MapMember(m => m.Name).SetElementName("Name");

            cm.SetIgnoreExtraElements(true);
        });

        return services;
    }
}
