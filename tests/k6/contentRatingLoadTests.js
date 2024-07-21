
import { uuidv4, randomIntBetween, randomString } from 'https://jslib.k6.io/k6-utils/1.4.0/index.js';

import http from "k6/http";
import { group, check, sleep } from "k6";

export let options = {
    scenarios: {
        my_web_test: {

          
          executor: 'constant-vus',
          vus: 5,
          duration: '10m',
        },
      },
    // insecureSkipTLSVerify: true,
    // // vus: 1,
    // duration: '10s',
    // iterations: 1,
    // noConnectionReuse: false
  };
  const BASE_URL = "https://localhost:7247"
// Sleep duration between successive requests.
// You might want to edit the value of this variable or remove calls to the sleep function on the script.
const SLEEP_DURATION = 0.3;
// Global variables should be initialized.



export default function () {
    let headers = { 'Content-Type': 'application/json', 'Authorization': `${__ENV.APIKEY}`, "Accept": "application/json"};
    let listId = uuidv4();
    postContentEstimationListEditor(listId, headers);
    postContentEstimationListEditorNewContent(randomIntBetween(2,30), listId, headers);
    const contentListEditor = getContentListEditor(listId, headers);
    putContentListEditorContent(listId, contentListEditor.content[0], headers);
    getContentListEditors(10, headers);
    deleteContentFromList(listId, contentListEditor.content[1].id, headers)

    let roomId = postContentPartyEstimationRoom(listId, headers);
    sleep(SLEEP_DURATION);
    getContentPartyEstimationRooms(headers);
    sleep(SLEEP_DURATION);
    let room = getContentPartyEstimationRoom(roomId, headers);
    putEstimateContentList(room.contentRatings, room.raters[0].id,headers)

    // group("createlisteditor", () => {
        
    //     postContentEstimationListEditor(listId, headers);
    //     postContentEstimationListEditorNewContent(randomIntBetween(2,30), listId, headers);
    //     const contentListEditor = getContentListEditor(listId, headers);
    //     putContentListEditorContent(listId, contentListEditor.content[0], headers);
    //     getContentListEditors(10, headers);
    //     deleteContentFromList(listId, contentListEditor.content[1].id, headers)
    // });
    // group("estimateContent", () => {
    //     let roomId = postContentPartyEstimationRoom(listId, headers);
    //     sleep(SLEEP_DURATION);
    //     getContentPartyEstimationRooms(headers);
    //     sleep(SLEEP_DURATION);
    //     let room = getContentPartyEstimationRoom(roomId, headers);
    //     putEstimateContentList(room.contentRatings, room.raters[0].id,headers)
        
    // });

}
function postContentEstimationListEditor(contentListId, headers){
    let url = BASE_URL + `/api/content-estimation-list-editor`;
    let body = {"id": contentListId, "roomName": randomString(5)};
    let params = { headers };
    let request = http.post(url, JSON.stringify(body), params);
    check(request, {
        "Created": (r) => r.status === 201
    });
}
function postContentEstimationListEditorNewContent(count, contentListId, headers){
    let url = BASE_URL + `/api/content-estimation-list-editor/${contentListId}/content`;
    let params = { headers };
    for(let i = 0; i < count; i++){  
    let contentName = randomString(5)
    const payload = JSON.stringify({
        id: uuidv4(),
        name: contentName,
        url: BASE_URL + "/" + contentName,
        contentType: 'Video',
      });
      let request = http.post(url, payload, params);
      check(request, {
        "Created": (r) => r.status === 201
    });
    sleep(1);
}
}
function getContentListEditor(listId, headers){
    let url = BASE_URL + `/api/content-estimation-list-editor/${listId}`;
    let params = { headers };
    const res = http.get(url, params);
    check(res, {
        'status was 200': (r) => r.status === 200,
      });
      const responseObject = JSON.parse(res.body);
      return responseObject; 
}
function putContentListEditorContent(listId, content, headers){
    let url = BASE_URL + `/api/content-estimation-list-editor/${listId}/content/${content.id}`;
    let params = { headers };
    let contentName = randomString(5);
    const payload = JSON.stringify({
        name: contentName,
        url: BASE_URL + "/" + contentName,
        contentType: 'Audio',
      });
      let request = http.put(url, payload, params);
      check(request, {
        'status was 200': (r) => r.status === 200,
      });
}
function deleteContentFromList(listId, contentId, headers) {
    let url = BASE_URL + `/api/content-estimation-list-editor/${listId}/content/${contentId}`;
    let params = { headers };
    let request = http.del(url, null, params);
    check(request, {
        'status was 204': (r) => r.status === 204,
      });
}
function getContentListEditors(getCount, headers){
    let url = BASE_URL + `/api/content-estimation-list-editor`;
    let params = { headers };
    for(let i = 0; i < getCount; i++){  
    const res = http.get(url, params);
    check(res, {
        'status was 200': (r) => r.status === 200,
      });
      sleep(SLEEP_DURATION)
    }
}
function getContentPartyEstimationRooms(headers) {
    let url = BASE_URL + `/api/content-party-estimation-room`;
    let params = { headers };
    const res = http.get(url, params);
    check(res, {
        "OK": (r) => r.status === 200
    });
}
function getContentPartyEstimationRoom(roomId ,headers) {
    let url = BASE_URL + `/api/content-party-estimation-room/${roomId}`;
    let params = { headers };
    const res = http.get(url, params);
    check(res, {
        'status was 200': (r) => r.status === 200,
      });
      const responseObject = JSON.parse(res.body);
      return responseObject; 
}
function postContentPartyEstimationRoom(contentListId, headers){
    let roomId = uuidv4()
    let url = BASE_URL + `/api/content-party-estimation-room`;
    const payload = JSON.stringify({
        roomId: roomId,
        contentListId: contentListId,
        roomName: uuidv4(),
        minRating: 0,
        maxRating: 10,
      });
      let params = { headers };
      const res = http.post(url, payload, params);
      check(res, {
        "Created": (r) => r.status === 201
    });
    return roomId;
}
function putEstimateContentList(contentList, raterId, headers){
    let params = { headers };
    
    contentList.forEach((content)=>{
        let url = BASE_URL + `/api/content-party-rating/${content.ratingId}`;
        const payload = JSON.stringify({
            newScore: randomIntBetween(0, 10),
            raterForChangeScoreId: raterId,
          });
          let res = http.put(url, payload, params);
          check(res, {
            'status was 200': (r) => r.status === 200,
          });
          sleep(SLEEP_DURATION)
    });
}

