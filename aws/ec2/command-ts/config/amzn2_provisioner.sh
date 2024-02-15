#!/bin/bash

# **************** Install Docker/Ansible Engine on Amazon Linux 2 ****************

required_packages_installation(){
    sudo amazon-linux-extras install epel -y
    sudo amazon-linux-extras enable epel
}

docker_installation(){
    echo ">>>>>>>>>> Installing docker-ce using amazon-linux-extras"
    sudo amazon-linux-extras install docker -y
    sudo amazon-linux-extras enable docker -y
    
    # Start the Docker service and enable it to start after system reboot
    sudo systemctl start docker
    # To join the docker group that is allowed to use the docker daemon
    sudo usermod -aG docker ec2-user

    # Restart the docker daemon
    sudo systemctl restart docker

    # Verify the status of Docker service
    sudo systemctl status docker
    
    echo "docker is now installed on your Amazon Linux system"
}

ansible_installation(){
  echo ">>>>>>>>>> Installing Ansible"
  sudo yum install ansible -y

  echo ">>>>>>>>>> Ansible Version"
  ansible --version

  echo "Ansible is now installed on your Amazon Linux system"
}

PROVISIONED_ON=/etc/vm_provision_on_timestamp
if [ -f "$PROVISIONED_ON" ]
then
  echo "VM was already provisioned at: $(cat $PROVISIONED_ON)"
  echo "To run system updates manually login via 'vagrant ssh' and run 'yum update -y'"
  exit
fi

# Update package list and upgrade all packages
sudo yum update -y

# Required packages Installation
required_packages_installation

# Docker Installation
docker_installation

# Ansible Installation
ansible_installation

# Tag the provision time:
date > "$PROVISIONED_ON"

echo "Successfully created Ansible Engine virtual machine."
