import { DaprServer } from "@dapr/dapr";

import ScrapperActor from "./scrapper-actor";

const SERVER_PORT = process.env.PORT || process.env.SERVER_PORT || "5002";

async function main() {
  const server = new DaprServer(undefined, SERVER_PORT);

  await server.actor.init(); // Let the server know we need actors
  await server.actor.registerActor(ScrapperActor); // Register the actor
  await server.start();
  console.log(`scrapper-actor::started on port ${SERVER_PORT}`)
}

main().catch((e) => console.error(e));
