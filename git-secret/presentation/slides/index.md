- title : Git Secret
- description : 
- author : Karel Šťastný
- theme : night 
- transition : none

***

# Git Secret

## Karel Šťastný

***

## TODO Issues with secrets

DESCRIBE the drawbacks of storing secrets separately



    These files are not version controlled. Filenames, locations, and passwords change from time to time, or new information appears, and other information is removed. When secrets are stored separately from your repo, you can not tell for sure which version of the configuration file was used with each commit or deploy.

    When building the automated deployment system there will be one extra step: download and place these secret-configuration files where they need to be. This also means you have to maintain extra secure servers where all your secrets are stored.


***  

## TODO Solution - Git Secret

DESCRIBE what git secret does


    git-secret encrypts files and stores them inside your git repository, providing a history of changes for every commit.
    git-secret doesn’t require any extra deploy operations other than providing the appropriate private key (to allow decryption), and using git secret reveal to decrypt all the secret files.


***

## TODO Asymmetic Cryptography

JUST IMAGE, do not go into details

***
## Install (Debian systems) I

Install the prerequisities

```bash
sudo apt install gnupg make man git gawk
```

Install git-secret

```bash
sudo apt install git-secret
```

https://web.archive.org/web/20220703064608/https://git-secret.io/installation

' at the moment, original domain git-secret.io is not working due to sanctions

just for sharing the scripts, do not go through the setup here

***

## Install (Debian systems) II

```bash
sudo sh -c "echo 'deb https://gitsecret.jfrog.io/artifactory/git-secret-deb git-secret main' >> /etc/apt/sources.list"
wget -qO - 'https://gitsecret.jfrog.io/artifactory/api/gpg/key/public' | sudo apt-key add -
sudo apt-get update && sudo apt-get install -y git-secret

git secret --version
```

***

## GPG

DESCRIBE what it is

***

## GPG generating keys

```bash
gpg --gen-key
```

```bash
gpg --armor --export your.email@address.com > public-key.gpg
```

***

```bash
gpg --import public-key.gpg
```


***

## Git secret Usage

initialize

tell

add 

hide

reveal

***

## Example

> Sharing files with git secret


***

## The Good

- solved sharing of secrets
- simple usage
- can be used as part of CI/CD pipeline

'not demonstrated, we are not using this

***

## The Bad

- no direct Windows support for those to whom this matters
- reveal needs to run from root .git directory, sometimes confusing


***

## The Ugly

- error messages not very helpful
- secrets cannot be merged - developers need to be careful not to overwrite

***

## Sources

Slides at https://github.com/kstastny/Talks

* https://sobolevn.me/git-secret/
* https://github.com/sobolevn/git-secret
* https://gnupg.org/index.html





*********** REMOVE STUFF BELOW
## John H. Conway

![](images/20090310_ConwayKochen_DJA_066-copy.jpg)

<p class="reference">Photo by
Denise Applewhite, Office of Communications, Princeton.edu</p>  

' https://www.princeton.edu/news/2020/04/14/mathematician-john-horton-conway-magical-genius-known-inventing-game-life-dies-age

***

## Game of Life

![](images/Gospers_glider_gun.gif)

<p class="reference"><a href="https://commons.wikimedia.org/wiki/File:Gospers_glider_gun.gif">https://commons.wikimedia.org/wiki/File:Gospers_glider_gun.gif</a></p>  

***

## Rules

 1. Any live cell with two or three live neighbours survives.
 1. Any dead cell with three live neighbours becomes a live cell.
 1. All other live cells die in the next generation. Similarly, all other dead cells stay dead.

***

## Implementation

***  

## Sources

* You can find this talk on my github https://github.com/kstastny/Talks
* https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life
* [John Conway on Game of Life](https://www.youtube.com/watch?v=R9Plq-D1gEk)
* https://fable.io/docs/
