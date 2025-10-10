# Doppler Tracking

The [SGP4](https://celestrak.org/publications/AIAA/2006-6753/)
algorithm used in SkyRoof to compute the Doppler offset produces very accurate results for the LEO
satellites, typically within tens of Hertz, if it receives accurate input data. For best results, ensure
that the following conditions are met.

## Home Location

Make sure that your grid square is accurate. Correct it in the
[Settings window](settings_window.md) if necessary.

## System Time

Your system clock should be accurate to a second. Get one of those little programs that run in the
system tray and periodically synchronize your clock with the time servers on the Internet.
[NetTime](https://www.timesynctool.com/) is one such program.

## PPM correction

Find the PPM correction factor of your SDR radio as described in the
[Calibrating PPM Correction](calibrating_ppm_correction.md) section and enter it in
[SDR settings](setting_up_sdr.md).

## TLE Data

SkyRoof downloads the TLE data automatically every 24 hours. Some sources claim that TLE may be updated
once a week, but that would not be enough for accurate Doppler tracking, especially for the satellites
that perform frequent orbit corrections. When in doubt, download TLE manually as described in the
[Satellite Data](satellite_data.md) section.

## Transmitter Frequency Correction

Most satellite transmitters transmit on the frequencies that differ from the nominal values by up to
a few kHz. A one-time correction described in the
[Frequency Control](frequency_control.md) and [Frequency Scale](frequency_scale.md) sections eliminates
this error.
