using System.Text;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using SmartInfluence.Collector.Extentions;
using SmartInfluence.Collector.YouTube;

Console.OutputEncoding = Encoding.UTF8;

var settings = Settings.LoadElasticSettings();

var channels = await YouTubeApi.CollectUkrainianChannelsAsync(Settings._requestModel);
