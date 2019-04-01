# Em-PrisonJobs-FiveM

Em-PrisonJobs is a FiveM plugin that introduces two jobs at Bolingbroke penitentiary. It's written in C# and is licensed under the [CC0 1.0 Universal](https://creativecommons.org/publicdomain/zero/1.0/).

## Jobs

- Food Delivery
- Electrician

## Events handled by PrisonJobs

`JailTimeOver`

Stops current animations, despawns any created props and resets job data. Triggering this event is recommended but it won't break anything if you forget.

## Events Triggered by PrisonJobs

`ReduceJailTime`

Sends a positive int with the time that should be reduced off of a players current sentence.

`addMoney`

Sends a positive int with the money that should given to the player.

`ShowInformationLeft`

Sends the time in ms to show a message and the message itself.

## Contributing

Contributions must be explicitly licensed under the CC0 1.0 Universal.