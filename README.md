# Em-PrisonJobs-FiveM

Em-PrisonJobs is a FiveM plugin that introduces two jobs at Bolingbroke penitentiary. It's written in C# and is licensed under the [CC0 1.0 Universal](https://creativecommons.org/publicdomain/zero/1.0/).

## Jobs

- Food Delivery
- Electrician

## Dependencies

<a href="https://github.com/Davenport-Physics/Em-PlayExternalSounds-FiveM">PlayExternalSounds</a>

## Events handled by PrisonJobs

`JailTimeOver`

Stops current animations, despawns any created props and resets job data. Triggering this event is recommended but it won't break anything if you forget.

## Events Triggered by PrisonJobs

`ReduceJailTime(int time_to_remove)`

Sends a positive int with the time that should be reduced off of a players current sentence.

`addMoney(int money_to_receive)`

Sends a positive int with the money that should given to the player.

`ShowInformationLeft(int ms, string message)`

Sends the time in ms to show a message and the message itself.

## Contributing

Contributions must be explicitly licensed under the CC0 1.0 Universal.