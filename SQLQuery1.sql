Create Table ResumeCommentInfo
(
Id varchar(38) primary key,
PostName varchar(100) not null,
InterviewDate dateTime not null,
InterviewFeedBack varchar(500) not null,
FeedBackDate datetime not null,
UserName varchar(50)
)
 