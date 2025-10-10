# SkyCAT  v.1.4

> [!Note]
> The documentation for SkyCAT is under development, these are just some notes for the beta testers.

## Overview

SkyCAT is a new CAT control engine, open source and multi-platform. It is available as a .NET assembly that developers can include in their software, and also as a command-line program that accepts TCP commands compatible with **rigctld.exe**.

## Supported Radios

This is the very first version of SkyCAT, it includes the command set files only for these radios:

- IC-9700
- IC-705
- IC-905
- IC-706MKIIG
- FT-847
- FT-817
- TS-2000

Once the software is fully tested, the files for other radios will be added. Moreover, the users will be able to create their own files, just as in OmniRig.

Quick tests have been performed by the beta testers with all radio models except FT-817. Please help me by testing SkyCAT with 817/818.

Detailed tests, as described below in the **What to Test** section, still need to be performed with all radios. Again, I rely on you and other users for this since I do not have all those radios and cannot do the tests myself. Please help me with this if you can.

## Running skycatd.exe

Download: [skycat.zip](http://www.dxatlas.com/other/skycat_1.4.zip)

There is no installer, just unzip all files in some folder.

Use the same command line parameters as you used with rigctld.exe, e.g.:

``` text
skycatd.exe -m 3081 -r COM9 -s 115200 -vvv -f
```

- the **-s** parameter is optional, the program knows the default speed of each radio;
- the **-vvv** option enables detailed logging;
- the new **-f** option enables writing the log to a file.

## Skycatd on Linux and Max

1. Install .NET 9 Runtime

- On macOS:

  ``` bash
  brew install dotnet
  ```
  
  - On Linux (Ubuntu):
  
  ``` bash
  sudo apt-get update
  sudo apt-get install -y dotnet-sdk-9.0
  ```
  
  More distros [here](https://learn.microsoft.com/dotnet/core/install/linux).

2. Run the program:

    ```text
    dotnet skycatd.dll
    ```


## Testing

Please test SkyCAT with SkyRoof first. Later you can try it with other programs, such as GPredict.

### Configuration

In the Settings dialog of SkyRoof, under CAT Control:

- **Ignore Dial Knob**: set to false;
- **Delay**: initially set to 300-1000 ms: with the default value of 100 ms it writes too many entries to the log. When you proceed to testing the dial knob, set it back to 100 ms for faster response (or longer than that if your radio cannot keep the pace);
- **Radio Type**: set to Duplex for IC-9700 and FT-847, and to Simplex for all other radios - both in the  RX and TX sections;
- **Host**: if you are running skycatd on another computer, enter the Host name or IP address of that computer.

Turn on the radio, run skycatd.exe, run SkyRoof - in any order.

### What to Test

- test the program with:
  - RX CAT and TX CAT enabled;
  - only TX enabled;
  - only RX enabled.

- test with different satellite transmitters:
  - SSB transponder;
  - FM repeater;
  - Digital (e.g., SONATE), where the uplink and downlink frequencies are on the same band;
  - Telemetry or Beacon that has only a downlink frequency;
  - switch between the V/U and U/V satellites to see if the band change works.

  Note that you do not have to wait for the satellite to rise, Doppler tracking works even if the satellite is not visible
- functions to test:
  - the rx and tx frequencies should change in the radio with time as the Doppler offsets change;
  - setting the rx and tx mode in SkyRoof should change the mode in the radio;
  - tuning the rx frequency in SkyRoof with a mouse should change the rx frequency in the radio;
  - changing the Uplink Manual Correction should change the tx frequency in the radio;
  - spinning the dial knob should change the frequency in SkyRoof:
    - if an SSB transponder is tracked, it should change the offset within the transponder segment;
    - for as FM repeater, the Downlink manual correction should change;
    - when transmitting, the dial knob should change the Uplink Manual Correction setting.
  - press the Transmit button in the radio and see if the Transmit button in SkyRoof changes the text;
  - press the Transmit button in SkyRoof to enable and disable transmission.


### Limitations

> [!Note]
> Many radios cannot write or even read the frequency and mode, or at least change the band, when transmitting.  This poses a problem in the Simplex mode because in this mode the software is responsible for writing the RX and TX frequency/mode to the radio when it switches from RX to TX and back. However, once the radio is in the TX mode, it is too late to set any parameters. If you see that your transceiver does not switch to the TX frequency when you press PTT in the radio, see if it works when you switch PTT using the Transmit button in SkyRoof, in this case the program has a chance to  write the settings before the radio goes to the transmit mode.

## Reporting The Results

Please let me know how skycat.exe works with your radio. If there are any issues, please describe them in detail. Please include your log files located in the folder where you put SkyCAT, in the Logs sub-folder. Send me the logs even if all works fine.

## Good to Know

### TCP Commands

The following TCP commands are supported by skycatd.exe:

- -f and -F
- -i and -I
- -m and -M
- -x and -X
- -t and -T
- -U

See the [rigctld documentation](https://hamlib.sourceforge.net/html/rigctld.1.html) for the  description of these commands.

### Operating Modes

Many radios have different CAT commands for the same operation, depending on the operating mode of the radio. An example is the Set Frequency command that is usually different in the SAT and VFO modes. Existing CAT software often uses wrong commands because it does not know the current operating mode. SatCAT sets this mode explicitly, so it knows which commands to use. The **-U** command is used to set up the radio in one of its operating modes:

- -U Duplex
- -U Split
- -U Simplex
- -U Transmitter
- -U Receiver

For compatibility with rigctld.exe, the '**U SATMODE 1**', '**S 1 VFOB**' and '**S 0 VFOB**' commands are recognized as aliases of the Duplex, Split and Simplex setup commands respectively.
