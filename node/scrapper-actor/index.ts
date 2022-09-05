import { DaprServer } from "@dapr/dapr";

import ScrapperActor from "./scrapper-actor";

const DAPR_HOST = process.env.DAPR_HOST || "http://localhost";
const DAPR_HTTP_PORT = process.env.DAPR_HTTP_PORT || "3502";
const SERVER_HOST = process.env.SERVER_HOST || "127.0.0.1";
const SERVER_PORT = process.env.SERVER_PORT || "5002";

async function main() {
  const server = new DaprServer(SERVER_HOST, SERVER_PORT, DAPR_HOST, DAPR_HTTP_PORT);

  await server.actor.init(); // Let the server know we need actors
  await server.actor.registerActor(ScrapperActor); // Register the actor
  await server.start();
}

main().catch((e) => console.error(e));
