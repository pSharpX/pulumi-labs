using System.Threading.Tasks;
using Pulumi;
using Docker = Pulumi.Docker;

class OneBankStack: Stack
{
    public OneBankStack()
    {
        var containerLabels = new ContainerLabelBuilder().ContainerLabels;

        var assessmentManagamentNetwork = new Docker.Network("AssessmentManagementNetwork", new Docker.NetworkArgs
        {
            Name = "onebank/AssessmentManagementNetwork"
        });
        var trackSuggestionNetwork = new Docker.Network("TrackSuggestionNetwork", new Docker.NetworkArgs
        {
            Name = "onebank/TrackSuggestionNetwork"
        });
        var discoveryNetwork = new Docker.Network("OneBank_DiscoveryNetwork", new Docker.NetworkArgs
        {
            Name = "onebank/DiscoveryNetwork"
        });
        var serviceDiscoveyNetwork = new Docker.Network("OneBank_ServiceDiscoveyNetwork", new Docker.NetworkArgs
        {
            Name = "onebank/ServiceDiscoveyNetwork"
        });
        var apiGatewayNetwork = new Docker.Network("OneBank_APIGatewayNetwork", new Docker.NetworkArgs
        {
            Name = "onebank/ApiGatewayNetwork"
        });
        var publicNetwork = new Docker.Network("OneBank_PublicNetwork", new Docker.NetworkArgs
        {
            Name = "onebank/PublicNetwork"
        });


        var mariaDbImage = new Docker.RemoteImage("MariaDb_Image", new Docker.RemoteImageArgs
        {
            Name = "mariadb:10.7.3",
            KeepLocally = true
        });
        var serviceDiscoveryImage = new Docker.RemoteImage("OneBank_ServiceDiscovery_Image", new Docker.RemoteImageArgs
        {
            Name = "psharpx/sfit-servicediscovery:latest",
            KeepLocally = true
        });
        var apiGatewayImage = new Docker.RemoteImage("OneBank_APIGateway_Image", new Docker.RemoteImageArgs
        {
            Name = "psharpx/sfit-apigateway:latest",
            KeepLocally = true
        });
        var mlTrackClassifierImage = new Docker.RemoteImage("OneBank_MlTrackClassifier_Image", new Docker.RemoteImageArgs
        {
            Name = "psharpx/sfit-ml-track-suggestion-api:latest",
            KeepLocally = true
        });
        var trackSuggestionImage = new Docker.RemoteImage("OneBank_TrackSuggestionAPI_Image", new Docker.RemoteImageArgs
        {
            Name = "psharpx/sfit-track-suggestion-api:latest",
            KeepLocally = true
        });
        var assessmentManagementImage = new Docker.RemoteImage("OneBank_AssessmentManagementAPI_Image", new Docker.RemoteImageArgs
        {
            Name = "psharpx/sfit-assessment-management-api:latest",
            KeepLocally = true
        });
        var discoveryApiImage = new Docker.RemoteImage("OneBank_DiscoveryAPI_Image", new Docker.RemoteImageArgs
        {
            Name = "psharpx/sfit-discovery-api:latest",
            KeepLocally = true
        });
        var swaggerImage = new Docker.RemoteImage("SwaggerUI_Image", new Docker.RemoteImageArgs
        {
            Name = "swaggerapi/swagger-ui:latest",
            KeepLocally = true
        });


        var  assessmentManagementDbContainer = new Docker.Container("OneBank_AssessmentManagementDb_Container", new Docker.ContainerArgs
        {
            Image = mariaDbImage.RepoDigest,
            Hostname = "assessment.database.sfit.pe",
            Ports = new InputList<Docker.Inputs.ContainerPortArgs>
            {
                new Docker.Inputs.ContainerPortArgs
                {
                    Internal = 3306,
                    External = 3306
                }
            },
            Labels = containerLabels,
            Envs = new InputList<string> 
            {
                "MARIADB_USER_FILE=/run/secrets/db_user",
                "MARIADB_PASSWORD_FILE=/run/secrets/db_password",
                "MARIADB_ROOT_PASSWORD_FILE=/run/secrets/db_password",
                "MARIADB_DATABASE=sfit-assessment-db",
            },
            NetworksAdvanced = new InputList<Docker.Inputs.ContainerNetworksAdvancedArgs>
            {
                new Docker.Inputs.ContainerNetworksAdvancedArgs
                {
                    Name = assessmentManagamentNetwork.Name
                }
            }
        });
        var  trackSuggestionDbContainer = new Docker.Container("OneBank_TrackSuggestionDb_Container", new Docker.ContainerArgs
        {
            Image = mariaDbImage.RepoDigest,
            Hostname = "track.database.sfit.pe",
            Ports = new InputList<Docker.Inputs.ContainerPortArgs>
            {
                new Docker.Inputs.ContainerPortArgs
                {
                    Internal = 3306,
                    External = 3307
                }
            },
            Labels = containerLabels,
            Envs = new InputList<string> 
            {
                "MARIADB_USER_FILE=/run/secrets/db_user",
                "MARIADB_PASSWORD_FILE=/run/secrets/db_password",
                "MARIADB_ROOT_PASSWORD_FILE=/run/secrets/db_password",
                "MARIADB_DATABASE=sfit-track-db",
            },
            NetworksAdvanced = new InputList<Docker.Inputs.ContainerNetworksAdvancedArgs>
            {
                new Docker.Inputs.ContainerNetworksAdvancedArgs
                {
                    Name = trackSuggestionNetwork.Name
                }
            }
        });
        var  discoveryDbContainer = new Docker.Container("OneBank_DiscoveryDb_Container", new Docker.ContainerArgs
        {
            Image = mariaDbImage.RepoDigest,
            Hostname = "business.database.sfit.pe",
            Ports = new InputList<Docker.Inputs.ContainerPortArgs>
            {
                new Docker.Inputs.ContainerPortArgs
                {
                    Internal = 3306,
                    External = 3308
                }
            },
            Labels = containerLabels,
            Envs = new InputList<string> 
            {
                "MARIADB_USER_FILE=/run/secrets/db_user",
                "MARIADB_PASSWORD_FILE=/run/secrets/db_password",
                "MARIADB_ROOT_PASSWORD_FILE=/run/secrets/db_password",
                "MARIADB_DATABASE=sfit-business-db",
            },
            NetworksAdvanced = new InputList<Docker.Inputs.ContainerNetworksAdvancedArgs>
            {
                new Docker.Inputs.ContainerNetworksAdvancedArgs
                {
                    Name = discoveryNetwork.Name
                }
            }
        });
        var  serviceDiscoveryContainer = new Docker.Container("OneBank_ServiceDiscovery_Container", new Docker.ContainerArgs
        {
            Image = serviceDiscoveryImage.RepoDigest,
            Hostname = "servicediscovery.sfit.pe",
            Ports = new InputList<Docker.Inputs.ContainerPortArgs>
            {
                new Docker.Inputs.ContainerPortArgs
                {
                    Internal = 8010,
                    External = 8010
                }
            },
            Restart = "always",
            Labels = containerLabels,
            Envs = new InputList<string> 
            {
                "SPRING_PROFILES_ACTIVE=test",
            },
            NetworksAdvanced = new InputList<Docker.Inputs.ContainerNetworksAdvancedArgs>
            {
                new Docker.Inputs.ContainerNetworksAdvancedArgs
                {
                    Name = serviceDiscoveyNetwork.Name
                }
            }
        });
        
        var  apiGatewayContainer = new Docker.Container("OneBank_APIGateway_Container", new Docker.ContainerArgs
        {
            Image = apiGatewayImage.RepoDigest,
            Hostname = "apigateway.sfit.pe",
            Ports = new InputList<Docker.Inputs.ContainerPortArgs>
            {
                new Docker.Inputs.ContainerPortArgs
                {
                    Internal = 8082,
                    External = 8082
                }
            },
            Restart = "always",
            Labels = containerLabels,
            Envs = new InputList<string> 
            {
                "SPRING_PROFILES_ACTIVE=test",
                "SERVICEDISCOVERY_HOSTNAME=servicediscovery.sfit.pe"
            },
            NetworksAdvanced = new InputList<Docker.Inputs.ContainerNetworksAdvancedArgs>
            {
                new Docker.Inputs.ContainerNetworksAdvancedArgs
                {
                    Name = apiGatewayNetwork.Name
                },
                new Docker.Inputs.ContainerNetworksAdvancedArgs
                {
                    Name = serviceDiscoveyNetwork.Name
                },
                new Docker.Inputs.ContainerNetworksAdvancedArgs
                {
                    Name = assessmentManagamentNetwork.Name
                },
                new Docker.Inputs.ContainerNetworksAdvancedArgs
                {
                    Name = trackSuggestionNetwork.Name
                },
                new Docker.Inputs.ContainerNetworksAdvancedArgs
                {
                    Name = discoveryNetwork.Name
                }
            }
        }, new CustomResourceOptions
        {
            DependsOn = serviceDiscoveryContainer
        });

        Url = Output.Create("http://localhost:8080/");
    }

    [Output]
    public Output<string> Url {get; set;}
}