-- Table: Greek

--DROP TABLE Greek;

CREATE TABLE Greek
(
  book_name nvarchar(30) not null, -- Long name of book
  short_name nvarchar(10) not null,
  canon_order smallint not null default -1, -- Protestant Canonical sequence number....
  chapter smallint not null default -1,
  verse smallint not null default -1,
  sentence_position smallint not null default -1, -- Order of this word in this sentence
  book_position int not null default -1, -- Position of this word in this book. This way we can measure distance between words. Each book is like its own number line.
  adjective bit not null default 0,
  conjunction bit not null default 0,
  adverb bit not null default 0,
  interjection bit not null default 0,
  noun bit not null default 0,
  preposition bit not null default 0,
  article bit not null default 0,
  demons_pronoun bit not null default 0, -- demonstrative pronoun
  indef_pronoun bit not null default 0, -- interrogative/indefinite pronoun
  person_pronoun bit not null default 0, -- personal pronoun
  relative_pronoun bit not null default 0, -- relative pronoun
  verb bit not null default 0,
  particle bit not null default 0,
  person smallint not null default -1, -- person (1=1st, 2=2nd, 3=3rd)
  tense nvarchar(30), -- tense (P=present, I=imperfect, F=future, A=aorist, X=perfect, Y=pluperfect)
  voice nvarchar(30), -- voice (A=active, M=middle, P=passive)
  mood nvarchar(30), -- mood (I=indicative, D=imperative, S=subjunctive, O=optative, N=infinitive, P=participle)
  noun_case nvarchar(30), -- case (N=nominative, G=genitive, D=dative, A=accusative)
  sing_or_plural char(1), -- S = singular...
  gender char(1), -- M=masculine, F=feminine, N=neuter
  degree char(1), -- degree (C=comparative, S=superlative)
  word_with_punct nvarchar(80), -- The word, with sentence markers like question marks, etc.
  word_without_punct nvarchar(80), -- The word without any question marks or such on the end.
  word nvarchar(80), -- The with normalzed accents and such. Good for searching on exact hits.
  root nvarchar(80), -- Root word
  strongs_num int not null default -1,
  family_num int not null default -1, -- Each word can belong to a family, and that family is a number. This is for speed searching on relationships. This number will be based on how words group and relate.
  root_num int not null default -1, -- Every unique Greek root gets a number, to speed searching
  root_freq_nt int not null default -1, -- How often does the root word appear in the Greek NT
  word_freq_nt int not null default -1, -- How often this word appears as is in whole NT
  root_freq_book int not null default -1, -- How often this root word appears in this book
  word_freq_book int not null default -1 -- How often this word as this word like this word in this form appears in this book
)

ALTER TABLE Greek
ADD CONSTRAINT [unique_constraint_greek] UNIQUE(short_name, book_position)

--WITH (
--  OIDS=FALSE
--);
--ALTER TABLE Greek
  --OWNER TO postgres;
--COMMENT ON TABLE Greek
--  IS 'Any books of the Bible in Greek. NT or LXX';
--COMMENT ON COLUMN Greek.book_name IS 'Long name of book';
--COMMENT ON COLUMN Greek.canon_order IS 'Protestant Canonical sequence number.
--There are 66 books of the bible. Which is this one?';
--COMMENT ON COLUMN Greek.demons_pronoun IS 'demonstrative pronoun';
--COMMENT ON COLUMN Greek.indef_pronoun IS 'interrogative/indefinite pronoun';
--COMMENT ON COLUMN Greek.person_pronoun IS 'personal pronoun';
--COMMENT ON COLUMN Greek.relative_pronoun IS 'relative pronoun';
--COMMENT ON COLUMN Greek.person IS 'person (1=1st, 2=2nd, 3=3rd)';
--COMMENT ON COLUMN Greek.tense IS 'tense (P=present, I=imperfect, F=future, A=aorist, X=perfect, Y=pluperfect) ';
--COMMENT ON COLUMN Greek.voice IS 'voice (A=active, M=middle, P=passive)';
--COMMENT ON COLUMN Greek.mood IS 'mood (I=indicative, D=imperative, S=subjunctive, O=optative, N=infinitive, P=participle)';
--COMMENT ON COLUMN Greek.noun_case IS 'case (N=nominative, G=genitive, D=dative, A=accusative)';
--COMMENT ON COLUMN Greek.sing_or_plural IS 'S = singular
--P = plural';
--COMMENT ON COLUMN Greek.gender IS 'M=masculine, F=feminine, N=neuter';
--COMMENT ON COLUMN Greek.degree IS 'degree (C=comparative, S=superlative)';
--COMMENT ON COLUMN Greek.word_with_punct IS 'The word, with sentence markers like question marks, etc.';
--COMMENT ON COLUMN Greek.word_without_punct IS 'The word without any question marks or such on the end.';
--COMMENT ON COLUMN Greek.word IS 'The with normalzed accents and such. Good for searching on exact hits.';
--COMMENT ON COLUMN Greek.root IS 'Root word';
--COMMENT ON COLUMN Greek.sentence_position IS 'Order of this word in this sentence';
--COMMENT ON COLUMN Greek.family_num IS 'Each word can belong to a family, and that family is a number. This is for speed searching on relationships. This number will be based on how words group and relate.';
--COMMENT ON COLUMN Greek.root_num IS 'Every unique Greek root gets a number, to speed searching';
--COMMENT ON COLUMN Greek.book_position IS 'Position of this word in this book. This way we can measure distance between words. Each book is like its own number line.';
--COMMENT ON COLUMN Greek.root_freq_nt IS 'How often does the root word appear in the Greek NT';
--COMMENT ON COLUMN Greek.word_freq_nt IS 'How often this word appears as is in whole NT';
--COMMENT ON COLUMN Greek.root_freq_book IS 'How often this root word appears in this book';
--COMMENT ON COLUMN Greek.word_freq_book IS 'How often this word as this word like this word in this form appears in this book';

