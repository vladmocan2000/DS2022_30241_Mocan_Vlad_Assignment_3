import "./Chat.css";
import { useEffect, useState } from "react";
import { Client, ChatMessage} from "./../../grpc/chat1_pb";
import { ChatServiceClient } from "../../grpc/chat1_pb_service";

const chatService = new ChatServiceClient("http://localhost:5000", null, null);
console.log("dddddddddddd");
export default function Chat({currentUser, username, to}) {

  const [message, setMessage] = useState('');

  const [msgList, setMsgList] = useState([]);
  const [typing, setTyping] = useState(false);
  const [read, setRead] = useState(false);
  var a = 0;
  useEffect(() => {

    if(username === currentUser && a === 0) {
      a++;console.log(a);
      const client = new Client();
      client.setUsername(currentUser + "->" + to);

      var chatStream = chatService.receiveMsg(client);
      chatStream.on("data", (response) => {

        const from = response.getFrom();
        const msg = response.getMsg();
        const to = response.getTo();
        console.log("Typing:" + response.getTyping());
        console.log("Read:" + response.getRead());
        if(currentUser === to) {

          setTyping(response.getTyping());
          setRead(response.getRead())
        }
        else if(msg !== ""){

          setRead(false);
        }

          setTimeout(() => {
            setTyping(false);
          }, 1000);

        console.log(msg)
        if (from === currentUser && msg !== "") {

          setMsgList((oldArray) => [...oldArray, { from, to, msg, mine: true }]);

        } else if(msg !== "") {

          setMsgList((oldArray) => [...oldArray, { from, to, msg, mine: false }]);
        }
      });

      chatStream.on("status", function (status) {
        console.log(status.code, status.details, status.metadata);
      });

      chatStream.on("end", () => {
        console.log("Stream ended.");
      });
    }
  }, []);

  function sendMessage(message, typing, read, to) {

    const msg = new ChatMessage();
    msg.setMsg(message);
    msg.setFrom(currentUser);
    msg.setTo(to);
    msg.setTyping(typing);
    msg.setRead(read);
    console.log(msg);
    chatService.sendMsg(msg, null, (err, response) => {

      console.log(response);
    });
  }

  const handleMessageChange = event => {
    
    sendMessage("", true, true, to);
    setMessage(event.target.value);
  };

  const handleClick = event => {
    
    sendMessage("", false, true, to);
  };

  function handler() {
    console.log(to);
    sendMessage(message, false, true, to);
    setMessage("");
  }

  return (

    <div className="chat">

      <div className="chat-header">
        <h3>Chat to {to}</h3>
      </div>

      <div className="chat-list">
        {msgList?.map((chat, i) => (chat.from === to || chat.to === to) && (<ChatCard chat={chat} key={i} />))}
      </div>

      <div className="chat-input">
        <div style={{ flex: "3 1 90%" }}>
          <textarea id={to} value={message} onChange={handleMessageChange} onFocus={handleClick} />
        </div>
        <div
          style={{
            paddingLeft: "5px",
            display: "flex",
            alignItems: "center",
            justifyContent: "flex-end",
          }}
        >
          <button onClick={handler}>Send</button>
        </div>
      </div>
      {
        (typing) &&
        (<div>Typing</div>)
      }
      {
        (read) &&
        (<div>Read</div>)
      }
    </div>
  );
}

function ChatCard({ chat }) {
  return (
    <>
      <div style={{ fontSize: "9px", marginLeft: "4px", paddingLeft: "8px" }}>
        <span>{chat?.from}</span>
      </div>
      <div
        className={
          chat?.mine ? "chatcard chatcard-mine" : "chatcard chatcard-friend"
        }
      >
        <div className="chatcard-msg">
          <span>{chat?.msg}</span>
        </div>
      </div>
    </>
  );
}