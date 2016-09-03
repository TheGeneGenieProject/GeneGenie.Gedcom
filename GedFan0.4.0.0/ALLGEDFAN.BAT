@ECHO OFF
REM USAGE: ALLGEDFAN %1
REM %1 = largest number of generations
FOR /L %%I IN (1,1,%1) DO GEDFAN /G:%%I /L:EN


