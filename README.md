# CreateNuixNistDigestList
Generates a Nuix digest list using NIST modern Reference Data Set (RDS) hash set file.

# Features
The following features are supported;
- Downloads NSRL hash sets directly from NIST NSRL download site.
- Specify version of NSRL hash sets to download (current is default)
- Abilitiy to specify which hash set to use during digest file creation..
- Creates a Nuix compatible digest list file using entries from an RDS hash set file.

# RDS Hash Sets
The latest RDS hash sets can be downloaded from the following page:

https://www.nist.gov/itl/ssd/software-quality-group/national-software-reference-library-nsrl/nsrl-download/current-rds

Typically, the Modern RDS (unique) set is used.

# Testing
The test folder contains a sample Modern RDS hash set test file. Use this file to test the application to create a Nuix digest list.


# Console Usage

There is a version of CreateNuixNistDigest that runs in the Windows console. The following command-line options are available:

```
CreateNuixNistDigest [-version version] [-url url] [-help] [-skipdownload]

  -version      Specifies the version of RDS to download.
                Uses current if not specified.
  -url          Specifies the root url to download files from.
                Uses https://s3.amazonaws.com/rds.nsrl.nist.gov/RDS if not specified.
                This parameter should never change.
  -skipdownload specifies the root url to download files from.
  -delhashcodes delete and recreated hashcode files.
  -modern       include modern RDS hash set.
  -modernm      include modern minimal RDS hash set (default).
  -modernu      include modern unique RDS hash set.
  -android      include android RDS hash set (default).
  -ios          include iOS RDS hash set (default)
  -legacy       include Legacy RDS hash set.

  -help         Displays this help text.

```

## Creating a Nuix Digest List Using Current RDS Hash Set

Creating a Nuix digest list using the most current RDS Hash Set is the simpliest use case. Just run the program without any options.

## Create a Nuix Digest List Using a Specific RDS Hash Set

To create a Nuix digest list using a specific version of the RDS Hash Set, specify the version using the *-version* option followed by the version number. A typical RDS Hash Set version number is formatted as MM.mm (e.g. 2.71).

The following example will create a digest list using version 2.71 of the RDS Hash Set:

```
CreateNuixDigestListDirect -version 2.71
```
## Notable Behaviors
The following are notable application behaviors:

### Download is automatically skipped. 

If a downloaded file exists, the download is skipped regardless of what the Skip Download setting is set to.

### Hashcode operation maybe be continued

If a chunk file exists, it assumes that it was part of an incomplete hashcode extract operation from a given NSRL text file. The number of hashcode entries is calculated in the chunk file and skips that number of lines in the given NSRL text file. This allows for the hashcode extract operation to continue where it has left off.

Note that it is advisable to delete the existing chunk file with the highest serial number prior re-running. This is to avoid an event where the last file written during the previous operation was not completely written.

### Display current RDS version

The application provides the most current version of the RDS set.
