﻿using System.Collections.Generic;

namespace lite_test.Infrastructure.AppSettings
{
    public class CosmosDbSettings
    {
        public string EndpointUrl { get; set; }

        public string PrimaryKey { get; set; }

        public string DatabaseName { get; set; }

        public List<ContainerInfo> Containers { get; set; }

    }
    public class ContainerInfo
    {
        public string Name { get; set; }

        public string PartitionKey { get; set; }
    }

}
