# CAT command notes

While working on the SkyCAT engine, I discovered that the documentation for the CAT commands of many radios is incomplete and sometimes wrong.
The only way to learn the actual behavior of the CAT system is by experimentation. Below are my notes that may help those who are trying to use CAT commands in their software.

## IC-9700

- **"16", "59", "00"** - this command that disables the dual watch mode sometimes returns success code **"FB"** and sometimes the error code, **"FA"**. In both cases dual watch is disabled. I could not find any pattern in its response, just be prepared to ignore the error;

- **""05"** - set the operating frequency, fails in the SAT mode if the frequency is on the same band as that of the Sub receiver. The work around is, when it fails, to send the Swap A/B command, then set the frequency again. Note that the modes of the Main and Sub receive also get swapped and need to be restored;

- **""05"** - if Split is enabled and we are setting the VFO frequency that is on a band other than the band of the other VFO, this command disables Split. However, if another frequency change returns to the previous band, the Split gets automatically enabled! It remembered all this time that we wanted Split, and enabled it when the VFO A and B were tuned to the same band.

## FT-847

- all CAT commands are ignored until the client sends the CAT On command (5 zero bytes);

- this radio has the Split mode, and even allows it to be used in a cross-band manner, but does not have a CAT command to enable Split, nor does it even have a command to set the VFO B frequency or at least swap the VFO's.

## FT-817

- **"F7"** - the TX status returned by this command has a PTT on/off flag in its 7-th bit. This is stated in the docs, but the values used for ON and OFF are not documented. It turns out that the logic is inverted here, 0 means ON and 1 means OFF;

- this radio returns a 1-byte reply to the Set commands, but this is not documented. The returned values are different, but it is not clear what they mean.

## IC-706MKIIG

- supports split operation, but the only way to change the TX frequency is by swapping the VFO's. Useless for the SAT work;

- does not have commands to read or set PTT.

## IC-910

- has SAT mode but no way to set the frequency of the other VFO;

- **19**, **01** - "Set satellite memory". No description of the data format is provided. Actually the format of the parameters is not described for many commands, should be inferred from other radios;

- the CAT command table in the older versions of Instruction Manual is split in rows incorrectly, the command parameters and their descriptions do not match. Get the latest revision of the Manual from the ICOM web site.
