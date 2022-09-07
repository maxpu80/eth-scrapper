import { AbstractActor } from "@dapr/dapr";
import axios from "axios";
import { Data as HandlerData, Error, handle, Result, Success } from "./handler";

//

interface RequestBlockRange {
  from?: number;
  to?: number;
}

interface Data {
  contractAddress: string;
  abi: string;
  blockRange: RequestBlockRange;
}

const mapRequestBlockRange = (range: RequestBlockRange) => ({
  from: range.from ? { Some: [range.from] } : null,
  to: range.to ? { Some: [range.to] } : null,
});

const mapPublishResultSuccess = (requestBlockRange: RequestBlockRange, result: Success) => {
  return {
    Ok: [
      {
        events: result.events,
        blockRange: result.blockRange,
        requestBlockRange: mapRequestBlockRange(requestBlockRange),
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
const mapPublishResultError = (requestBlockRange: RequestBlockRange, result: Error) => {
  return {
    Error: [
      {
        data: mapPublishResultErrorData(result),
        blockRange: result.blockRange,
        requestBlockRange: mapRequestBlockRange(requestBlockRange),
      },
    ],
  };
};

const mapPublishResult = (requestBlockRange: RequestBlockRange, result: Result) => {
  switch (result.kind) {
    case "Success":
      return mapPublishResultSuccess(requestBlockRange, result);
    case "Error":
      return mapPublishResultError(requestBlockRange, result);
  }
};

const mapPublishPayload = (data: Data, result: Result) => {
  return {
    contractAddress: data.contractAddress,
    abi: data.abi,
    result: mapPublishResult(data.blockRange, result),
  };
};

export interface IScrapperActor {
  scrap(data: Data): Promise<any>;
}

export default class ScrapperActor extends AbstractActor implements IScrapperActor {
  private async invokeActor(actorType: string, actorMethod: string, payload: any) {
    const client = this.getDaprClient();

    let actorId = this.getActorId().getId();

    const url = `${client.daprHost}:${client.daprPort}/v1.0/actors/${actorType}/${actorId}/method/${actorMethod}`;

    await axios.put(url, payload);

    return payload;
  }

  private async publishError(data: Data, result: Result) {
    const payload = mapPublishPayload(data, result);

    return this.invokeActor("scrapper-dispatcher", "Continue", payload);
  }

  private async publishSuccess(data: Data, result: Success) {
    /*
    const payload = {
      events: result.events,
      blockRange: result.blockRange,
      requestBlockRange: mapRequestBlockRange(data.blockRange),
    };
    */
    const _payload = mapPublishPayload(data, result);
    const payload = {
      ..._payload,
      result: (_payload.result as any).Ok[0]
    }


    return this.invokeActor("scrapper-store", "Store", payload);
  }

  private async publish(data: Data, result: Result) {
    switch (result.kind) {
      case "Success":
        return this.publishSuccess(data, result);
      case "Error":
        return this.publishError(data, result);
    }
  }

  async scrap(data: Data) {
    console.log("scrapper::scrap::start", data.blockRange);

    const abi = JSON.parse(data.abi);

    const handlerData: HandlerData = {
      contractAddress: data.contractAddress,
      abi,
      blockRange: data.blockRange,
    };

    const result = await handle(handlerData);

    console.log("scrapper::scrap::result", result.blockRange, result.kind === "Success" ? "Success" : result.error);

    const publishedResult = await this.publish(data, result);

    console.log("scrapper::scrap::publish", publishedResult.contractAddress);

    return publishedResult;
  }
}
