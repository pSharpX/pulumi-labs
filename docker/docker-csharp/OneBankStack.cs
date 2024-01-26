using Pulumi;
using Docker = Pulumi.Docker;

class OneBankStack: Stack
{
    public OneBankStack()
    {
        var tag = "0.0.1-SNAPSHOT";
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


        var mariaDbImage = new Docker.RemoteImage("MariaDb_Image", new Docker.RemoteImageArgs
        {
            Name = "mariadb:10.7.3",
            KeepLocally = true
        });
        var serviceDiscoveryImage = new Docker.RemoteImage("OneBank_ServiceDiscovery_Image", new Docker.RemoteImageArgs
        {
            Name = $"psharpx/sfit-servicediscovery:{tag}",
            KeepLocally = true
        });
        var apiGatewayImage = new Docker.RemoteImage("OneBank_APIGateway_Image", new Docker.RemoteImageArgs
        {
            Name = $"psharpx/sfit-apigateway:{tag}",
            KeepLocally = true
        });
        var mlTrackClassifierImage = new Docker.RemoteImage("OneBank_MlTrackClassifier_Image", new Docker.RemoteImageArgs
        {
            Name = "psharpx/sfit-ml-track-suggestion-api:0.0.1",
            KeepLocally = true
        });
        var trackSuggestionImage = new Docker.RemoteImage("OneBank_TrackSuggestionAPI_Image", new Docker.RemoteImageArgs
        {
            Name = $"psharpx/sfit-track-suggestion-api:{tag}",
            KeepLocally = true
        });
        var assessmentManagementImage = new Docker.RemoteImage("OneBank_AssessmentManagementAPI_Image", new Docker.RemoteImageArgs
        {
            Name = $"psharpx/sfit-assessment-management-api:{tag}",
            KeepLocally = true
        });
        var discoveryApiImage = new Docker.RemoteImage("OneBank_DiscoveryAPI_Image", new Docker.RemoteImageArgs
        {
            Name = $"psharpx/sfit-discovery-api:{tag}",
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
                "MARIADB_USER=sfit-user",
                "MARIADB_PASSWORD=sfit-pass",
                "MARIADB_ROOT_PASSWORD=sfit-pass",
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
                "MARIADB_USER=sfit-user",
                "MARIADB_PASSWORD=sfit-pass",
                "MARIADB_ROOT_PASSWORD=sfit-pass",
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
                "MARIADB_USER=sfit-user",
                "MARIADB_PASSWORD=sfit-pass",
                "MARIADB_ROOT_PASSWORD=sfit-pass",
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
        var  mlTrackClassifierContainer = new Docker.Container("OneBank_MlTrackClassifier_Container", new Docker.ContainerArgs
        {
            Image = mlTrackClassifierImage.RepoDigest,
            Hostname = "track.classifier.sfit.pe",
            Restart = "always",
            Labels = containerLabels,
            NetworksAdvanced = new InputList<Docker.Inputs.ContainerNetworksAdvancedArgs>
            {
                new Docker.Inputs.ContainerNetworksAdvancedArgs
                {
                    Name = trackSuggestionNetwork.Name
                }
            }
        });
        var  assessmentManagementContainer = new Docker.Container("OneBank_AssessmentManagement_Container", new Docker.ContainerArgs
        {
            Image = assessmentManagementImage.RepoDigest,
            Hostname = "assessment.sfit.pe",
            Restart = "always",
            Labels = containerLabels,
            Envs = new InputList<string> 
            {
                "SPRING_PROFILES_ACTIVE=test",
                "DATABASE_HOSTNAME=assessment.database.sfit.pe",
                "DATABASE_NAME=sfit-assessment-db",
                "SPRING_DATASOURCE_USERNAME=sfit-user",
                "SPRING_DATASOURCE_PASSWORD=sfit-pass",
                "SERVICEDISCOVERY_HOSTNAME=servicediscovery.sfit.pe"
            },
            NetworksAdvanced = new InputList<Docker.Inputs.ContainerNetworksAdvancedArgs>
            {
                new Docker.Inputs.ContainerNetworksAdvancedArgs
                {
                    Name = serviceDiscoveyNetwork.Name
                },
                new Docker.Inputs.ContainerNetworksAdvancedArgs
                {
                    Name = assessmentManagamentNetwork.Name
                }
            }
        }, new CustomResourceOptions
        {
            DependsOn = new InputList<Resource>
            {
                assessmentManagementDbContainer,
                serviceDiscoveryContainer
            }
        });
        var  trackSuggestionContainer = new Docker.Container("OneBank_TrackSuggestion_Container", new Docker.ContainerArgs
        {
            Image = trackSuggestionImage.RepoDigest,
            Hostname = "track.sfit.pe",
            Restart = "always",
            Labels = containerLabels,
            Envs = new InputList<string> 
            {
                "SPRING_PROFILES_ACTIVE=test",
                "DATABASE_HOSTNAME=track.database.sfit.pe",
                "DATABASE_NAME=sfit-track-db",
                "SPRING_DATASOURCE_USERNAME=sfit-user",
                "SPRING_DATASOURCE_PASSWORD=sfit-pass",
                "SERVICEDISCOVERY_HOSTNAME=servicediscovery.sfit.pe",
                "TRACKCLASSIFIER_HOSTNAME=track.classifier.sfit.pe",
                "TRACKCLASSIFIER_PORT=80"
            },
            NetworksAdvanced = new InputList<Docker.Inputs.ContainerNetworksAdvancedArgs>
            {
                new Docker.Inputs.ContainerNetworksAdvancedArgs
                {
                    Name = serviceDiscoveyNetwork.Name
                },
                new Docker.Inputs.ContainerNetworksAdvancedArgs
                {
                    Name = trackSuggestionNetwork.Name
                }
            }
        }, new CustomResourceOptions
        {
            DependsOn = new InputList<Resource>
            {
                trackSuggestionDbContainer,
                serviceDiscoveryContainer,
                mlTrackClassifierContainer,
            }
        });
        var  discoveryContainer = new Docker.Container("OneBank_DiscoveryAPI_Container", new Docker.ContainerArgs
        {
            Image = discoveryApiImage.RepoDigest,
            Hostname = "business.sfit.pe",
            Restart = "always",
            Labels = containerLabels,
            Envs = new InputList<string> 
            {
                "SPRING_PROFILES_ACTIVE=test",
                "DATABASE_HOSTNAME=business.database.sfit.pe",
                "DATABASE_NAME=sfit-business-db",
                "SPRING_DATASOURCE_USERNAME=sfit-user",
                "SPRING_DATASOURCE_PASSWORD=sfit-pass",
                "SERVICEDISCOVERY_HOSTNAME=servicediscovery.sfit.pe"
            },
            NetworksAdvanced = new InputList<Docker.Inputs.ContainerNetworksAdvancedArgs>
            {
                new Docker.Inputs.ContainerNetworksAdvancedArgs
                {
                    Name = serviceDiscoveyNetwork.Name
                },
                new Docker.Inputs.ContainerNetworksAdvancedArgs
                {
                    Name = discoveryNetwork.Name
                }
            }
        }, new CustomResourceOptions
        {
            DependsOn = new InputList<Resource>
            {
                discoveryDbContainer,
                serviceDiscoveryContainer,
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
            DependsOn = new InputList<Resource>
            {
                serviceDiscoveryContainer,
                assessmentManagementContainer,
                trackSuggestionContainer,
                discoveryContainer
            }
        });

        Url = Output.Create("http://localhost:8082/");
    }

    [Output]
    public Output<string> Url {get; set;}
}