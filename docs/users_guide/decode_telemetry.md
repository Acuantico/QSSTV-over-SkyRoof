
# How to Decode Telemetry from PEARL-1C

I/Q or Audio data, streamed via VAC or UDP, may be used to decode telemetry transmitted by the satellites.
There is a number of telemetry decoders to choose from. One such decoder is
[gr_satellites.exe](https://gr-satellites.readthedocs.io/en/latest/command_line.html) command line tool,
its installation instructions are
[here](https://gr-satellites.readthedocs.io/en/latest/installation_conda.html).

---

This command runs gr_satellites.exe to decode telemetry of the PEARL-1C satellite using the I/Q UDP stream from SkyRoof:

```text
(base) C:\Ham>gr_satellites 58342 --udp --udp_port 7355 --udp_raw --iq --samp_rate 48e3 --hexdump
```

This command decodes the same satellite via I/Q output to VAC :

```text
(base) C:\Ham>gr_satellites 58342 --audio "CABLE Output (VB-Audio Virtual Cable)" --samp_rate 48000 --iq
```
To use the second command, you will need a virtual audio cable, such as
[VB-Audio](https://vb-audio.com/Cable/index.htm).

Run one of these commands, then in SkyRoof [settings](setting_up_output_stream.md):

- select either I/Q to VAC or I/Q to UDP, depending on the command you use;
- set Gain, dB to 0;
- select the VAC in the list of audio devices;
- click on the Output Stream label on the status bar to enable the output.

Example output from gr_satellites.exe v.5.7.0:

```text
pagesize :debug: Setting pagesize to 4096 B
top_block_impl :debug: Using default scheduler "TPB"
udp_source :info: Listening for data on UDP port 7355.
***** VERBOSE PDU DEBUG PRINT ******
((transmitter . 9k6 FSK downlink))
pdu length =         64 bytes
pdu vector contents =
0000: 9c 86 aa 8e a6 62 e0 a0 8a 82 a4 98 86 e1 03 f0
0010: f9 11 01 83 43 33 a9 e7 a4 10 00 00 11 00 00 00
0020: 00 00 00 00 7a 0f 01 00 00 00 00 00 00 00 00 00
0030: 05 00 00 00 77 fb 01 00 67 aa 00 00 00 00 00 00
************************************
***** VERBOSE PDU DEBUG PRINT ******
((transmitter . 9k6 FSK downlink))
pdu length =         88 bytes
pdu vector contents =
0000: 9c 86 aa 8e a6 62 e0 a0 8a 82 a4 98 86 e1 03 f0
0010: fb 11 01 81 43 33 00 00 00 00 00 00 00 00 00 00
0020: 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
0030: 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
0040: 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
0050: 00 00 00 00 00 00 00 00
************************************
***** VERBOSE PDU DEBUG PRINT ******
((transmitter . 9k6 FSK downlink))
pdu length =         56 bytes
pdu vector contents =
0000: 9c 86 aa 8e a6 62 e0 a0 8a 82 a4 98 86 e1 03 f0
0010: 06 11 01 82 43 33 83 8a 01 00 9e 00 00 00 04 00
0020: 00 00 88 ff f8 ae 00 00 80 01 00 00 0f 02 00 00
0030: 00 00 00 00 00 00 00 00
```
---
