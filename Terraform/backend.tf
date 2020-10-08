terraform {
  backend "remote" {
    organization = "phil-dev"

    workspaces {
      name = "teams-meeting-randomiser"
    }
  }
}