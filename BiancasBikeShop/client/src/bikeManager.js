const apiUrl = '/api/bike';

export const getBikes = () => {
    return fetch(`${apiUrl}`, {
        method: "GET",
    }).then((res) => res.json());
}

export const getBikeById = (id) => {
    return fetch(`${apiUrl}/${id}`, {
        method: "GET",
    }).then((res) => res.json());
}

export const getBikesInShopCount = () => {
    return fetch(`${apiUrl}/bikecount`, {
        method: "GET",
    }).then((res) => res.json());
}

// export const getAllReactions = () => {
//     return getToken().then((token) => {
//         return fetch(`${baseUrl}`, {
//             method: "GET",
//             headers: {
//                 Authorization: `Bearer ${token}`,
//             },
//         }).then((res) => res.json());
//     })
// };

// export const getReaction = (id) => {
//     return getToken().then((token) => {
//         return fetch(`${baseUrl}/${id}`, {
//             method: "GET",
//             headers: {
//                 Authorization: `Bearer ${token}`,
//             },
//         }).then((res) => res.json());
//     })
// };