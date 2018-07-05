import * as axios from "axios";

const URL_PREFIX = "";

export function passwordreset_getByToken(token) {
  return axios.get(URL_PREFIX + "/api/passwordreset?token=" + token);
}

export function passwordreset_post(data) {
  return axios.post(URL_PREFIX + "/api/passwordreset", data);
}

export function passwordreset_update(data) {
  return axios.put(URL_PREFIX + "/api/passwordreset", data);
}
