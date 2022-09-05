import { AbstractActor } from "@dapr/dapr";
import axios from "axios";
import { Data as HandlerData, Error, handle, Result, Success } from "./handler";

//
interface Data {
  contractAddress: string;
  abi: string;
  blockRange: { from?: number; to?: number };
}

const mapPublishResultSuccess = (result: Success) => {
  return {
    Ok: [
      {
        events: result.events,
        blockRange: result.blockRange,
      },
    ],
  };
};

const mapPublishResultErrorData = (result: Error) => {
  switch (result.error) {
    case "empty-result":
      return { EmptyResult: [] };
    case "limit-exceeded":
      return { LimitExceeded: [] };
    case "unknown":
      return { Unknown: [] };
  }
};
const mapPublishResultError = (result: Error) => {
  return {
    Error: [
      {
        data: mapPublishResultErrorData(result),
        blockRange: result.blockRange,
      },
    ],
  };
};

const mapPublishResult = (result: Result) => {
  switch (result.kind) {
    case "Success":
      return mapPublishResultSuccess(result);
    case "Error":
      return mapPublishResultError(result);
  }
};

const mapPublishPayload = (data: Data, result: Result) => {
  return {
    contractAddress: data.contractAddress,
    abi: data.abi,
    result: mapPublishResult(result),
  };
};

export interface IScrapperActor {
  scrap(data: Data): Promise<any>;
}

export default class ScrapperActor extends AbstractActor implements IScrapperActor {
  private async publish(data: Data, result: Result) {
    const client = this.getDaprClient();

    let actorId = this.getActorId().getId();

    const url = `${client.daprHost}:${client.daprPort}/v1.0/actors/scrapper-dispatcher/${actorId}/method/Continue`;

    const payload = mapPublishPayload(data, result);

    await axios.put(url, payload);

    //console.log("published !!!", url, payload);

    return payload;
  }

  async scrap(data: Data) {
    console.log("scrapper::scrap:", data);

    //const data = { ..._data, abi: _data.abi.replace(/\\"/g, '"') };

    const abi = JSON.parse(data.abi);

    const handlerData: HandlerData = {
      contractAddress: data.contractAddress,
      abi,
      blockRange: data.blockRange,
    };

    const result = await handle(handlerData);

    const publishedResult = await this.publish(data, result);

    return publishedResult;
  }
}
